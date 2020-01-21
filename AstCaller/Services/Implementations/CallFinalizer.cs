using AstCaller.Models.Domain;
using AstCaller.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AstCaller.Services.Implementations
{
    public class CallFinalizer : ICallFinalizer
    {
        private readonly IConfiguration _configuration;
        private readonly MainContext _context;
        private readonly ILogger<CallFinalizer> _logger;
        private static readonly Regex _regexStatus = new Regex(@"Status:\s*(\S+)");
        private static readonly Regex _regexFile = new Regex(@"call_(\d+)_(\d+)_([\dabcdef]{8}-[\dabcdef]{4}-[\dabcdef]{4}-[\dabcdef]{4}-[\dabcdef]{12}).call");

        private static Lazy<Dictionary<string, int>> _statusMap = new Lazy<Dictionary<string, int>>(() =>
         {
             var dict = new Dictionary<string, int>();
             dict.Add("Expired", 4);
             dict.Add("Completed", 2);
             dict.Add("Failed", 3);
             return dict;
         });

        public CallFinalizer(IConfiguration configuration, MainContext context, ILogger<CallFinalizer> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            var files = Directory.GetFiles(_configuration.GetValue<string>("Asterisk:FinishedCallFilesDir"), "call_*.call");
            foreach (var file in files)
            {
                await ProcessFinishedCallAsync(file);
            }
        }

        private async Task ProcessFinishedCallAsync(string file)
        {
            var fileMatch = _regexFile.Match(file);
            if (!fileMatch.Success)
                return;

            var campaignId = int.Parse(fileMatch.Groups[1].Value);
            var abonentId = int.Parse(fileMatch.Groups[2].Value);
            var uniqueId = Guid.Parse(fileMatch.Groups[3].Value);

            var fileContent = await File.ReadAllTextAsync(file);
            var match = _regexStatus.Match(fileContent);
            var status = 5;
            if (match.Success)
            {
                try
                {
                    status = _statusMap.Value[match.Groups[1].Value];
                }
                catch (KeyNotFoundException)
                {
                    _logger.LogError($"ProcessFinishedCallAsync:: Unexpected status: {match.Groups[1].Value}, file: ==={fileContent}===");
                }
            }

            var abonent = await _context.CampaignAbonents.FirstOrDefaultAsync(x => x.Id == abonentId);
            if (abonent == null)
            {
                _logger.LogError($"Could not find abonent with Id={abonentId}");
                return;
            }
            if (abonent.UniqueId != uniqueId)
            {
                _logger.LogError($"Wrong unique id in file {file}, possibly old data");
                return;
            }

            abonent.Status = status;
            abonent.CallInfo = fileContent;
            await _context.SaveChangesAsync();
            File.Delete(file);
        }

        public async Task CleanupAsync()
        {
            var abonents = _context.CampaignAbonents.Where(x => !x.HasErrors && (x.Status == null || x.Status == 1));

            var finishedCampaigns = await _context.Campaigns
                .Where(x => (x.Status == (int)CampaignViewModel.CampaignStatuses.Stopped || x.Status == (int)CampaignViewModel.CampaignStatuses.Running)
                && !abonents.Any(a => a.CampaignId == x.Id))
                .Select(x => x.Id).ToArrayAsync();
            if (finishedCampaigns.Length > 0)
            {
                foreach (var campaign in finishedCampaigns)
                {
                    var entity = await _context.Campaigns.FirstAsync(x => x.Id == campaign);
                    entity.Status = (int)CampaignViewModel.CampaignStatuses.Finished;
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
