using System;

namespace AstCaller.Models.Domain
{
    public class CampaignAbonent : BaseModel
    {
        public int CampaignId { get; set; }

        public string Phone { get; set; }

        public bool HasErrors { get; set; }

        public int? ModifierId { get; set; }

        public DateTime Modified { get; set; }

        public User Modifier { get; set; }

        public Campaign Campaign { get; set; }
    }
}
