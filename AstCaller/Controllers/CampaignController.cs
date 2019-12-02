using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.Classes;
using AstCaller.Models.Domain;
using AstCaller.Services;
using AstCaller.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AstCaller.Controllers
{
    public class CampaignController : BaseAuthorizedController
    {
        private readonly MainContext _context;
        private readonly string _uploadsDir;
        private readonly IAbonentsFileService _abonentsFileService;
        private readonly IConfiguration _configuration;

        public CampaignController(MainContext context,
            IAbonentsFileService abonentsFileService,
            IHostingEnvironment hostingEnvironment,
            ILogger<CampaignController> logger,
            IUserProvider userProvider,
            IConfiguration configuration) : base(logger, userProvider)
        {
            _context = context;
            _uploadsDir = Path.Combine(Directory.GetParent(hostingEnvironment.WebRootPath).FullName, "uploads");
            _abonentsFileService = abonentsFileService;
            _configuration = configuration;
        }

        public IActionResult Index(int page = 0)
        {
            ViewBag.Page = page;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id = null)
        {
            ViewBag.AsteriskActions = await _context.AsteriskExtensions.Where(x => !x.Disabled).Select(x => new AsteriskExtensionViewModel
            {
                Extension = x.Extension,
                Title = x.Title
            }).ToArrayAsync();

            if (id.HasValue)
            {
                var entity = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                {
                    throw new NullReferenceException($"Campaign with id={id} does not exists");
                }

                var schedules = await _context.CampaignSchedules.Where(x => x.CampaignId == id).ToArrayAsync();
                if (schedules.Length == 0)
                {
                    schedules = new CampaignSchedule[] { new CampaignSchedule {
                        DateStart = DateTime.Today,
                        DateEnd = DateTime.Today,
                        TimeStart = 480,
                        TimeEnd = 1260,
                        DaysOfWeek = 127
                    } };
                }

                return View(new CampaignViewModel
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    AbonentsTotal = entity.AbonentsCount,
                    AbonentsFileName = entity.AbonentsFileName,
                    VoiceFileName = entity.VoiceFileName,
                    Action = entity.Extension,

                    Schedules = schedules.Select(x => new CampaignScheduleViewModel
                    {
                        DateStart = x.DateStart,
                        DateEnd = x.DateEnd,
                        TimeStart = TimeToString(x.TimeStart),
                        TimeEnd = TimeToString(x.TimeEnd),
                        DaysOfWeek = x.DaysOfWeek
                    })
                });
            }

            return View(new CampaignViewModel
            {
                Action = "Play",
                Schedules = new CampaignScheduleViewModel[]
                {
                    new CampaignScheduleViewModel
                    {
                        DateStart = DateTime.Today,
                        DateEnd = DateTime.Today,
                        TimeStart = "8:00",
                        TimeEnd = "21:00",
                        DaysOfWeek = 127
                    }
                }
            });
        }

        private string TimeToString(int time)
        {
            return $"{time / 60}:{time % 60:00}";
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CampaignViewModel model)
        {
            ViewBag.AsteriskActions = await _context.AsteriskExtensions.Where(x => !x.Disabled).Select(x => new AsteriskExtensionViewModel
            {
                Extension = x.Extension,
                Title = x.Title
            }).ToArrayAsync();

            if (!model.Id.HasValue)
            {
                if (model.AbonentsFile == null)
                {
                    ModelState.AddModelError(nameof(model.AbonentsFile), "Для запуска обзвона необходим файл списка абонентов");
                }

                if (model.VoiceFile == null)
                {
                    ModelState.AddModelError(nameof(model.VoiceFile), "Для запуска обзвона необходима запись голосового сообщения");
                }

                if (model.Schedules == null || model.Schedules.Count() == 0)
                {
                    ModelState.AddModelError(nameof(model.Schedules), "Для запуска обзвона необходимо добавить расписание");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Не удалось сохранить форму, проверьте, заполнены ли все поля";
                _logger.LogWarning($"Campaign form validation error. Total errors: {ModelState.ErrorCount}, errors: [{string.Join(",", ModelState.Select(x => $"{x.Key}:{x.Value.ValidationState}"))}], model: [{JsonConvert.SerializeObject(model)}]");
                return View(model);
            }

            try
            {
                var entity = await GetOrCreateEntity(model.Id);

                entity.Name = model.Name;
                entity.Modified = DateTime.Now;
                entity.ModifierId = _currentUserId ?? 0;
                entity.Extension = model.Action;

                if (!model.Id.HasValue)
                {
                    await _context.SaveChangesAsync();
                    model.Id = entity.Id;
                }

                var schedules = await _context.CampaignSchedules.Where(x => x.CampaignId == entity.Id).ToArrayAsync();

                var toRemove = schedules.Where(x => !model.Schedules.Any(s => s.Id == x.Id));
                if (toRemove.Any())
                {
                    foreach (var schedule in toRemove)
                    {
                        _context.RemoveRange(toRemove);
                    }
                }

                foreach (var schedule in model.Schedules)
                {
                    var scEntity = schedules.FirstOrDefault(x => x.Id == schedule.Id);
                    if (scEntity == null)
                    {
                        scEntity = new CampaignSchedule
                        {
                            CampaignId = entity.Id
                        };
                        _context.CampaignSchedules.Add(scEntity);
                    }

                    scEntity.ModifierId = _currentUserId.Value;
                    scEntity.DateStart = schedule.DateStart;
                    scEntity.DateEnd = schedule.DateEnd;
                    scEntity.DaysOfWeek = schedule.DaysOfWeek;
                    scEntity.TimeStart = TimeFromString(schedule.TimeStart);
                    scEntity.TimeEnd = TimeFromString(schedule.TimeEnd);
                }

                if (model.AbonentsFile != null)
                {
                    await SaveFile(model.AbonentsFile, FileType.Abonents.ToFileName(entity.Id));
                    entity.AbonentsCount = await _abonentsFileService.ProcessFileAsync(Path.Combine(_uploadsDir, $"{entity.Id}_abonents"), entity.Id);
                    entity.AbonentsFileName = model.AbonentsFile.FileName;
                }

                if (model.VoiceFile != null)
                {
                    var voiceFileName = FileType.Voice.ToFileName(entity.Id);
                    await SaveFile(model.VoiceFile, voiceFileName);
                    entity.VoiceFileName = model.VoiceFile.FileName;
                    if (_configuration.GetValue<bool>("Asterisk:UseSox"))
                    {
                        Convert(voiceFileName);
                    }
                    else
                    {
                        System.IO.File.Copy(Path.Combine(_uploadsDir, voiceFileName), Path.Combine(_configuration.GetValue<string>("Asterisk:Sounds"), voiceFileName) + Path.GetExtension(model.VoiceFile.FileName));
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not save Campaign");
                ViewBag.Error = ex.Message;
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        private void Convert(string fileName)
        {
            try
            {
                var soxExecutable = _configuration.GetValue<string>("Asterisk:SoxPath");
                if (string.IsNullOrEmpty(soxExecutable))
                {
                    soxExecutable = "/bin/sox";
                }

                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = soxExecutable,
                        Arguments = $"{Path.Combine(_uploadsDir, fileName)} -r 8k -c 1 -b 16 {Path.Combine(_configuration.GetValue<string>("Asterisk:Sounds"), fileName)}.wav",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string result = process.StandardOutput.ReadToEnd();
                if (!string.IsNullOrEmpty(result))
                {
                    throw new Exception(result);
                }
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while executing sox convert file {fileName}");
                throw new Exception($"Не удалось обработать голосовой файл. Файл должен быть в формате wav");
            }
        }

        private int TimeFromString(string time)
        {
            var parts = time.Split(":");

            var hours = int.Parse(parts[0]);
            var minutes = int.Parse(parts[1]);
            return hours * 60 + minutes;
        }

        private bool[] DaysOfWeekToArray(int daysOfWeek)
        {
            return new bool[]
            {
                (daysOfWeek & 1) == 1,
                (daysOfWeek & 2) == 2,
                (daysOfWeek & 3) == 3,
                (daysOfWeek & 4) == 4,
                (daysOfWeek & 5) == 5,
                (daysOfWeek & 6) == 6,
                (daysOfWeek & 7) == 7,
            };
        }

        private int DaysOfWeekFromArray(bool[] daysOfWeek)
        {
            return daysOfWeek.Select((val, ind) => val ? (ind + 1) : 0).Sum();
        }

        private async Task<Campaign> GetOrCreateEntity(int? id)
        {
            if (id > 0)
            {
                return await _context.Campaigns.FirstAsync(x => x.Id == id);
            }

            var entity = new Campaign();
            _context.Campaigns.Add(entity);
            return entity;
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Campaigns.FirstAsync(x => x.Id == id);
            if (entity.Status != (int)CampaignViewModel.CampaignStatuses.Created)
            {
                throw new Exception("Невозможно удалить кампанию обзвона, которая была запущена ранее");
            }

            _context.Campaigns.Remove(entity);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> LoadData(int page = 1)
        {
            var countTask = _context.Campaigns.CountAsync();
            page--;
            if (page < 0)
            {
                page = 0;
            }

            var abonents = _context.CampaignAbonents;

            var query = _context.Campaigns.Select(x => new CampaignViewModel
            {
                Id = x.Id,
                AbonentsFileName = x.AbonentsFileName,
                AbonentsTotal = x.AbonentsCount,
                Name = x.Name,
                Status = (CampaignViewModel.CampaignStatuses)x.Status,
                VoiceFileName = x.VoiceFileName,
                AbonentsLoaded = abonents.Count(ca => ca.CampaignId == x.Id),
                AbonentsProcessed = abonents.Count(ca => ca.CampaignId == x.Id && ca.Status != 0),
                Modified = x.Modified
            });
            var data = await query
                .OrderByDescending(x => x.Modified)
                .Skip(page * 20)
                .Take(20)
                .ToArrayAsync();

            var campaignIds = data.Select(x => x.Id).ToArray();

            var schedules = await _context.CampaignSchedules.Where(x => campaignIds.Contains(x.CampaignId)).ToArrayAsync();
            Array.ForEach(data, x => x.Schedules = schedules.Where(s=>s.CampaignId == x.Id).Select(s => new CampaignScheduleViewModel
            {
                DateEnd = s.DateEnd,
                DateStart = s.DateStart,
                DaysOfWeek = s.DaysOfWeek,
                TimeEnd = TimeToString(s.TimeEnd),
                TimeStart = TimeToString(s.TimeStart)
            }).ToArray());

            var count = await countTask;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)count / 20);

            return PartialView(data);
        }

        [HttpGet]
        public async Task<IActionResult> Download(int campaignId, FileType fileType)
        {
            try
            {
                var fileName = fileType.ToFileName(campaignId);
                var campaignEntityTask = _context.Campaigns.FirstAsync(x => x.Id == campaignId);

                using (var memory = new MemoryStream())
                {
                    using (var stream = new FileStream(Path.Combine(_uploadsDir, fileName), FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    var campaignEntity = await campaignEntityTask;
                    return File(memory.ToArray(), "APPLICATION/octet-stream", campaignEntity.GetFileName(fileType));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Cannot download file {fileType.ToString()} for campaign {campaignId}");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Start(int id)
        {
            var campaignEntity = await _context.Campaigns.FirstAsync(x => x.Id == id);

            if (campaignEntity.Status == (int)CampaignViewModel.CampaignStatuses.Finished)
            {
                var errorText = $"Cannot start already finished campaign {id}, {campaignEntity.Name}";
                _logger.LogError(errorText);
                throw new Exception(errorText);
            }

            if (campaignEntity.Status == (int)CampaignViewModel.CampaignStatuses.Running)
            {
                return new JsonResult(new { success = true });
            }

            campaignEntity.Status = (int)CampaignViewModel.CampaignStatuses.Running;
            _context.SaveChanges();

            return new JsonResult(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Stop(int id)
        {
            var campaignEntity = await _context.Campaigns.FirstAsync(x => x.Id == id);

            if (campaignEntity.Status == (int)CampaignViewModel.CampaignStatuses.Created)
            {
                var errorText = $"Cannot stop campaign that was not started yet {id}, {campaignEntity.Name}";
                _logger.LogError(errorText);
                throw new Exception(errorText);
            }

            if (campaignEntity.Status == (int)CampaignViewModel.CampaignStatuses.Cancelled || campaignEntity.Status == (int)CampaignViewModel.CampaignStatuses.Finished)
            {
                var errorText = $"Cannot stop cancelled or finished campaign {id}, {campaignEntity.Name}";
                _logger.LogError(errorText);
                throw new Exception(errorText);
            }

            if (campaignEntity.Status == (int)CampaignViewModel.CampaignStatuses.Stopped)
            {
                return new JsonResult(new { success = true });
            }

            campaignEntity.Status = (int)CampaignViewModel.CampaignStatuses.Stopped;
            _context.SaveChanges();

            return new JsonResult(new { success = true });
        }

        private async Task SaveFile(IFormFile file, string saveName)
        {
            Directory.CreateDirectory(_uploadsDir);

            using (var stream = new FileStream(Path.Combine(_uploadsDir, saveName), FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
    }
}