using System;

namespace AstCaller.ViewModels.Reports
{
    public class ReportCampaignDetailAttemptViewModel
    {
        public DateTime CallDate { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public string Reason { get; set; }
    }
}
