using System;
using System.Collections.Generic;

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

        public Guid UniqueId { get; set; }

        public int? Status { get; set; }

        public string CallInfo { get; set; }

        public DateTime? CallStartDate { get; set; }

        public IEnumerable<CampaignAbonentHistory> CampaignAbonentHistories { get; set; }

        public CallStatus CallStatus { get; set; }
    }
}
