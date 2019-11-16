using System;
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
            if (id.HasValue)
            {
                var entity = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == id);
                return View(new CampaignViewModel
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    AbonentsTotal = entity.AbonentsCount,
                    AbonentsFileName = entity.AbonentsFileName,
                    VoiceFileName = entity.VoiceFileName
                });
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CampaignViewModel model)
        {
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
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var entity = await GetOrCreateEntity(model.Id);

                entity.Name = model.Name;
                entity.Modified = DateTime.Now;
                entity.ModifierId = _currentUserId ?? 0;

                if (!model.Id.HasValue)
                {
                    await _context.SaveChangesAsync();
                    model.Id = entity.Id;
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
                    System.IO.File.Copy(Path.Combine(_uploadsDir, voiceFileName), Path.Combine(_configuration.GetValue<string>("Asterisk:Sounds"), voiceFileName));
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
                AbonentsProcessed = abonents.Count(ca => ca.CampaignId == x.Id),
                Modified = x.Modified
            });
            var data = await query
                .OrderByDescending(x => x.Modified)
                .Skip(page * 20)
                .Take(20)
                .ToArrayAsync();

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