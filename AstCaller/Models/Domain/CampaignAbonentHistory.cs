using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstCaller.Models.Domain
{
    public class CampaignAbonentHistory : BaseModel
    {
        public int CampaignAbonentId { get; set; }

        public int? Status { get; set; }

        public DateTime CallDate { get; set; }

        public string Reason { get; set; }

        public CampaignAbonent CampaignAbonent { get; set; }

        public CallStatus CallStatus { get; set; }
    }
}
