using System.Collections.Generic;

namespace AstCaller.Models.Domain
{
    public class CallStatus : BaseModel
    {
        public string StatusName { get; set; }

        public IEnumerable<CampaignAbonentHistory> CampaignAbonentHistories { get; set; }

        public IEnumerable<CampaignAbonent> CampaignAbonents { get; set; }
    }
}
