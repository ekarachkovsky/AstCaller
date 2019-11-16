using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.Classes;
using AstCaller.Models;
using AstCaller.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AstCaller.Services.Implementations
{
    public class ScheduledProcessorService : IScheduledProcessorService
    {
        private readonly ILogger<ScheduledProcessorService> _logger;
        private IContextProvider _contextProvider;
        private ScheduleTaskModel _schedule;
        private readonly IConfiguration _configuration;
        private readonly int _lineLimit;
        private readonly string _trunkName;

        public ScheduledProcessorService(ILogger<ScheduledProcessorService> logger, IContextProvider contextProvider, IConfiguration configuration, ScheduleTaskModel schedule)
        {
            _logger = logger;
            _contextProvider = contextProvider;
            _schedule = schedule;
            _configuration = configuration;
            _lineLimit = _configuration.GetValue<int>("Asterisk:LinesLimit");
            _trunkName = _configuration.GetValue<string>("Asterisk:TrunkName");
        }

        public async Task ExecuteAsync()
        {
            await ProcessFinishedCallsAsync();
            var callsInProcess = CallsInProcess();
            if (callsInProcess > _lineLimit / 2)
            {
                return;
            }

            var abonents = await GetAbonentsAsync(callsInProcess);
            if (abonents.Count() == 0)
            {
                await FinishCampaignAsync();
                return;
            }

            foreach(var abonent in abonents)
            {
                await GenerateCallFile(abonent);
            }
        }

        private async Task GenerateCallFile(ScheduleTaskAbonentModel abonent)
        {
            var fileName = $"call_{_schedule.CampaignId}_{abonent.UniqueId}.call";
            var tempFile = Path.Combine(_configuration.GetValue<string>("Asterisk:TempDir"), fileName);
            await File.WriteAllTextAsync(tempFile,
                $"Channel: SIP/{_trunkName}/{abonent.Phone}" + Environment.NewLine +
                "MaxRetries: 3" + Environment.NewLine +
                "Application: Playback" + Environment.NewLine +
                $"Data: {FileType.Voice.ToFileName(_schedule.CampaignId)}" + Environment.NewLine +
                "Archive: Yes");

            File.Move(tempFile, Path.Combine(_configuration.GetValue<string>("Asterisk:CallFilesDir"), fileName));
            /*
             * Channel: SIP/voip_trunk/$number
Callerid: $number
MaxRetries: 200
RetryTime: 20
WaitTime: 30
Context: outboundmsg1
Extension: s
Priority: 1
              */
        }

        private async Task ProcessFinishedCallsAsync()
        {
            var files = Directory.GetFiles(_configuration.GetValue<string>("Asterisk:FinishedCallFilesDir"));
            foreach(var file in files)
            {
                await ProcessFinishedCallAsync(file);
            }
        }

        private async Task ProcessFinishedCallAsync(string file)
        {
            var content = await File.ReadAllTextAsync(file);

            //TODO: Do something
        }

        private async Task FinishCampaignAsync()
        {
            using (var context = _contextProvider.GetContext())
            {
                var campaign = await context.Campaigns.FirstAsync(x => x.Id == _schedule.CampaignId);
                campaign.Status = (int)CampaignViewModel.CampaignStatuses.Finished;
                await context.SaveChangesAsync();
            }
        }

        private int CallsInProcess()
        {
            return Directory.GetFiles(_configuration.GetValue<string>("Asterisk:CallFilesDir"), $"call_{_schedule.CampaignId}_*").Length;
        }

        private async Task<IEnumerable<ScheduleTaskAbonentModel>> GetAbonentsAsync(int callsInProcess)
        {
            using (var context = _contextProvider.GetContext())
            {
                var abonents = await context.CampaignAbonents.Where(x => x.CampaignId == _schedule.CampaignId &&
                        x.Status == 0 &&
                        !x.HasErrors)
                    .Select(x => new ScheduleTaskAbonentModel
                    {
                        Phone = x.Phone,
                        UniqueId = x.UniqueId
                    })
                    .Take(_lineLimit - callsInProcess)
                    .ToArrayAsync();

                return abonents;
            }
        }
    }
}
