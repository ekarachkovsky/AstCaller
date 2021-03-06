﻿using System.Collections.Generic;

namespace AstCaller.Models.Domain
{
    public class User : BaseModel
    {
        public string Fullname { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public IEnumerable<Campaign> Campaigns { get; set; }

        public IEnumerable<CampaignAbonent> CampaignAbonents { get; set; }

        public IEnumerable<CampaignSchedule> CampaignSchedules { get; set; }

        public IEnumerable<AsteriskExtension> AsteriskExtensions { get; set; }
        public bool IsDeleted { get; set; }
    }
}
