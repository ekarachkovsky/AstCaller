using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.Classes;
using AstCaller.Models;
using AstCaller.Models.Domain;
using AstCaller.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AstCaller.Services.Implementations
{
    public class ScheduledProcessorService : IScheduledProcessorService
    {
        private readonly ILogger<ScheduledProcessorService> _logger;
        private readonly MainContext _context;
        private readonly ScheduleTaskModel _schedule;
        private readonly IConfiguration _configuration;
        private readonly string _trunkName;

        public ScheduledProcessorService(ILogger<ScheduledProcessorService> logger, MainContext context, IConfiguration configuration, ScheduleTaskModel schedule)
        {
            _logger = logger;
            _context = context;
            _schedule = schedule;
            _configuration = configuration;
            _trunkName = _configuration.GetValue<string>("Asterisk:TrunkName");
        }

        public async Task ExecuteAsync()
        {
            var callsInProcess = CallsInProcess();
            _logger.LogInformation($"ScheduleProcessor({_schedule.CampaignId}) Current calls count: {callsInProcess}, limit: {_schedule.LineLimit}");

            if (callsInProcess > _schedule.LineLimit / 2)
            {
                return;
            }

            var abonents = await GetAbonentsAsync(callsInProcess);
            if (abonents.Count() == 0)
            {
                await FinishCampaignAsync();
                return;
            }

            foreach (var abonent in abonents)
            {
                await UpdateAbonentAsync(abonent);
                if (_schedule.Action == "Play")
                {
                    await GeneratePlayCallFile(abonent);
                }
                else
                {
                    await GenerateExtensionCallFile(abonent);
                }
            }
        }

        private async Task GenerateExtensionCallFile(ScheduleTaskAbonentModel abonent)
        {
            var fileName = $"call_{_schedule.CampaignId}_{abonent.Id}_{abonent.UniqueId}.call";
            var tempFile = Path.Combine(_configuration.GetValue<string>("Asterisk:TempDir"), fileName);
            await File.WriteAllTextAsync(tempFile,
                $"Channel: local/{abonent.Phone}@from-internal" + Environment.NewLine +
                "MaxRetries: 3" + Environment.NewLine +
                $"Context: {_schedule.Action}" + Environment.NewLine +
                "Extension: s" + Environment.NewLine +
                "Priority: 1" + Environment.NewLine +
                $"Set: audiofile=ru/{FileType.Voice.ToFileName(_schedule.CampaignId)}" + Environment.NewLine +
                $"Set: abonentid={abonent.Id}");
            //"Archive: Yes");

            File.Move(tempFile, Path.Combine(_configuration.GetValue<string>("Asterisk:CallFilesDir"), fileName));
        }

        private async Task UpdateAbonentAsync(ScheduleTaskAbonentModel abonent)
        {
            var entity = await _context.CampaignAbonents.FirstAsync(x => x.Id == abonent.Id);
            entity.Status = 1;
            entity.CallStartDate = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        private async Task GeneratePlayCallFile(ScheduleTaskAbonentModel abonent)
        {
            var fileName = $"call_{_schedule.CampaignId}_{abonent.Id}_{abonent.UniqueId}.call";
            var tempFile = Path.Combine(_configuration.GetValue<string>("Asterisk:TempDir"), fileName);
            await File.WriteAllTextAsync(tempFile,
                $"Channel: SIP/{_trunkName}/{abonent.Phone}" + Environment.NewLine +
                "MaxRetries: 3" + Environment.NewLine +
                "Application: Playback" + Environment.NewLine +
                $"Data: {FileType.Voice.ToFileName(_schedule.CampaignId)}" + Environment.NewLine +
                "Archive: Yes");

            File.Move(tempFile, Path.Combine(_configuration.GetValue<string>("Asterisk:CallFilesDir"), fileName));
        }

        private async Task FinishCampaignAsync()
        {
            if(await _context.CampaignAbonents.AnyAsync(x=>x.CampaignId==_schedule.CampaignId && x.Status == 1))
            {
                return;
            }

            var campaign = await _context.Campaigns.FirstAsync(x => x.Id == _schedule.CampaignId);
            campaign.Status = (int)CampaignViewModel.CampaignStatuses.Finished;
            await _context.SaveChangesAsync();
        }

        private int CallsInProcess()
        {
            return Directory.GetFiles(_configuration.GetValue<string>("Asterisk:CallFilesDir"), $"call_{_schedule.CampaignId}_*").Length;
        }

        private async Task<IEnumerable<ScheduleTaskAbonentModel>> GetAbonentsAsync(int callsInProcess)
        {
            var abonents = await _context.CampaignAbonents.Where(x => x.CampaignId == _schedule.CampaignId &&
                    (!x.Status.HasValue || x.Status < 1) &&
                    !x.HasErrors)
                .Select(x => new ScheduleTaskAbonentModel
                {
                    Id = x.Id,
                    Phone = x.Phone,
                    UniqueId = x.UniqueId
                })
                .Take(_schedule.LineLimit - callsInProcess)
                .ToArrayAsync();

            return abonents;
        }
    }
}
