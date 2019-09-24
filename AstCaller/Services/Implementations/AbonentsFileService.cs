using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.DataLayer;
using AstCaller.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace AstCaller.Services.Implementations
{
    public class AbonentsFileService : IAbonentsFileService
    {
        private readonly MainContext _context;
        private readonly IUserProvider _userProvider;

        public AbonentsFileService(MainContext context, IUserProvider userProvider)
        {
            _context = context;
            _userProvider = userProvider;
        }

        public async Task<int> ProcessFileAsync(string fileLocation, int campaignId)
        {
            var linesCount = 0;
            _context.CampaignAbonents.RemoveRange(await _context.CampaignAbonents.Where(x => x.CampaignId == campaignId).ToArrayAsync());
            using (var reader = new StreamReader(File.OpenRead(fileLocation)))
            {
                while (reader.Peek() >= 0)
                {
                    linesCount++;
                    ProcessLine(await reader.ReadLineAsync(), campaignId);
                }
            }
            await _context.SaveChangesAsync();

            return linesCount;
        }

        private void ProcessLine(string line, int campaignId)
        {
            var entity = new CampaignAbonent
            {
                CampaignId = campaignId,
                Phone = line,
                HasErrors = !long.TryParse(line, out _),
                ModifierId = _userProvider.Id,
                Modified = DateTime.Now
            };

            _context.CampaignAbonents.Add(entity);
        }
    }
}
