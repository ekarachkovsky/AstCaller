using AstCaller.Models.Domain;

namespace AstCaller.DataLayer.Implementations
{
    public class CampaignRepository : BaseRepository<Campaign>, ICampaignRepository
    {
        public CampaignRepository(ISqlConnectionProvider connectionProvider) : base(connectionProvider, "campaign")
        {
        }
    }
}
