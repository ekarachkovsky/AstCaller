using System;
using System.Collections;
using System.Collections.Generic;

namespace AstCaller.Models.Domain
{
    public class Campaign : BaseModel
    {
        public string Name { get; set; }

        public int Status { get; set; }

        public int AbonentsCount { get; set; }

        public string AbonentsFileName { get; set; }

        public string VoiceFileName { get; set; }

        public int ModifierId { get; set; }
        
        public DateTime Modified { get; set; }

        public User Modifier { get; set; }

        public IEnumerable<CampaignAbonent> CampaignAbonents { get; set; }

        public DateTime DateStart { get;  set; }

        public DateTime DateEnd { get;  set; }

        public int TimeStart { get;  set; }

        public int TimeEnd { get;  set; }

        public int DaysOfWeek { get; set; }
    }
}
