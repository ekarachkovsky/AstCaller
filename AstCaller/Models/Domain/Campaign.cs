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

        public IEnumerable<CampaignSchedule> CampaignSchedules { get; set; }

        public string Extension { get; set; }

        public AsteriskExtension AsteriskExtension { get; set; }
        
        public int LineLimit { get; set; }
        
        public bool IsDeleted { get; set; }

        public int? ClonedFromId { get; set; }

        public Campaign ClonedFrom { get; set; }
        public IEnumerable<Campaign> Clones { get; set; }
    }
}
