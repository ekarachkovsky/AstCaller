using System;
using System.IO;
using System.Threading.Tasks;
using AstCaller.Classes;
using AstCaller.DataLayer;
using AstCaller.Models.Domain;
using AstCaller.Services;
using AstCaller.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AstCaller.Controllers
{
    public class CampaignController : BaseAuthorizedController
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly string _uploadsDir;
        private readonly IAbonentsFileService _abonentsFileService;

        public CampaignController(ICampaignRepository campaignRepository,
            IAbonentsFileService abonentsFileService,
            IHostingEnvironment hostingEnvironment,
            ILogger<CampaignController> logger,
            IUserProvider userProvider) : base(logger, userProvider)
        {
            _campaignRepository = campaignRepository;
            _uploadsDir = Path.Combine(Directory.GetParent(hostingEnvironment.WebRootPath).FullName, "uploads");
            _abonentsFileService = abonentsFileService;
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
                var entity = await _campaignRepository.GetAsync(id.Value);
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
                var entity = new Campaign
                {
                    Id = model.Id ?? 0,
                    Name = model.Name,
                    Modified = DateTime.Now,
                    ModifierId = _currentUserId ?? 0
                };

                var id = await _campaignRepository.SaveAsync(entity);
                model.Id = id;
                if (model.AbonentsFile != null)
                {
                    await SaveFile(model.AbonentsFile, FileType.Abonents.ToFileName(id));
                }
                if (model.VoiceFile != null)
                {
                    await SaveFile(model.VoiceFile, FileType.Voice.ToFileName(id));
                }

                var count = await _abonentsFileService.ProcessFileAsync(Path.Combine(_uploadsDir, $"{id}_abonents"), id);

                if (count > 0)
                {
                    entity.Id = id;
                    entity.AbonentsCount = count;
                    await _campaignRepository.SaveAsync(entity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not save Campaign");
                ViewBag.Error = ex.Message;
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _campaignRepository.GetAsync(id);
            if (entity.Status != (int)CampaignViewModel.CampaignStatuses.Created)
            {
                throw new Exception("Невозможно удалить кампанию обзвона, которая была запущена ранее");
            }

            await _campaignRepository.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> LoadData(int page = 1)
        {
            var countTask = _campaignRepository.TotalCount();
            page--;
            if (page < 0)
            {
                page = 0;
            }

            var data = await _campaignRepository.GetRange<CampaignViewModel>(page * 20, 20, "vcampaigns");
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
                var campaignEntityTask = _campaignRepository.GetAsync(campaignId);

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
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Cannot download file {fileType.ToString()} for campaign {campaignId}");
                throw;
            }
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