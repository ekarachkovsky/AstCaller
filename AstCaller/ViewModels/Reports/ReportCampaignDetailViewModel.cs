using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstCaller.ViewModels.Reports
{
    public class ReportCampaignDetailViewModel
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public int? LastStatus { get; set; }
        public string LastStatusName { get; set; }
        public DateTime? CallStartDate { get; set; }
        public IEnumerable<ReportCampaignDetailAttemptViewModel> Attempts { get; set; }
    }
}
