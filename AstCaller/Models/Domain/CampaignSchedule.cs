using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstCaller.Models.Domain
{
    public class CampaignSchedule : BaseModel
    {
        public int CampaignId { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public int TimeStart { get; set; }

        public int TimeEnd { get; set; }

        public int DaysOfWeek { get; set; }

        public Campaign Campaign { get; set; }

        public int ModifierId { get; set; }

        public User Modifier { get; set; }
    }
}
