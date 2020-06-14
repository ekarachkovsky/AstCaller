﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstCaller.Models
{
    public class ScheduleTaskModel
    {
        public int CampaignId { get; set; }
        public string Action { get; set; }
        public int LineLimit { get; set; }
        public int Retries { get; set; }
    }
}
