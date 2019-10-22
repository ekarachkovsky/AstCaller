using System.Collections.Generic;

namespace AstCaller.Models.Domain
{
    public class User : BaseModel
    {
        public string Fullname { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public IEnumerable<Campaign> Campaigns { get; set; }

        public IEnumerable<CampaignAbonent> CampaignAbonents { get; set; }
    }
}
