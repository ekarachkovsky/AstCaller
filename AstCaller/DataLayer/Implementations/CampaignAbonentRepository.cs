using AstCaller.Models.Domain;

namespace AstCaller.DataLayer.Implementations
{
    public class CampaignAbonentRepository : BaseRepository<CampaignAbonent>, ICampaignAbonentRepository
    {
        public CampaignAbonentRepository(ISqlConnectionProvider connectionProvider) : base(connectionProvider, "campaignabonent")
        {
        }
    }
}
