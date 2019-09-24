using System;
using System.IO;
using System.Threading.Tasks;
using AstCaller.DataLayer;
using AstCaller.Models.Domain;

namespace AstCaller.Services.Implementations
{
    public class AbonentsFileService : IAbonentsFileService
    {
        private readonly ICampaignAbonentRepository _campaignAbonentRepository;
        private readonly IUserProvider _userProvider;

        public AbonentsFileService(ICampaignAbonentRepository campaignAbonentRepository, IUserProvider userProvider)
        {
            _campaignAbonentRepository = campaignAbonentRepository;
            _userProvider = userProvider;
        }

        public async Task<int> ProcessFileAsync(string fileLocation, int campaignId)
        {
            var linesCount = 0;
            using (var reader = new StreamReader(File.OpenRead(fileLocation)))
            {
                while (reader.Peek() >= 0)
                {
                    linesCount++;
                    await ProcessLineAsync(await reader.ReadLineAsync(), campaignId);
                }
            }

            return linesCount;
        }

        private async Task ProcessLineAsync(string line, int campaignId)
        {
            var entity = new CampaignAbonent
            {
                CampaignId = campaignId,
                Phone = line,
                HasErrors = !long.TryParse(line, out _),
                ModifierId = _userProvider.Id,
                Modified = DateTime.Now
            };

            await _campaignAbonentRepository.SaveAsync(entity);
        }
    }
}
