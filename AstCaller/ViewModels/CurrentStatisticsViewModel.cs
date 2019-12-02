using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstCaller.ViewModels
{
    public class CurrentStatisticsViewModel
    {
        public int ActiveCampaigns { get;  set; }
        public int CurrentRunningCampaigns { get;  set; }
        public int AbonentsInProcess { get;  set; }
        public int AbonentsInProcessStuck { get;  set; }
        public int AbonentsToCall { get; set; }
    }
}
