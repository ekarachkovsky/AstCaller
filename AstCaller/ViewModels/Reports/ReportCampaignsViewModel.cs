using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstCaller.ViewModels.Reports
{
    public class ReportCampaignsViewModel
    {
        public ReportCampaignStatsViewModel Stats { get; set; }
        public IEnumerable<ReportCampaignDetailViewModel> Details { get; set; }
    }
}
