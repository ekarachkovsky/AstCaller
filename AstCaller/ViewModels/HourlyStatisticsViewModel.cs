using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstCaller.ViewModels
{
    public class HourlyStatisticsViewModel
    {
        public Dictionary<int, string> Statuses { get; set; }
        public Dictionary<int,HourlyStatisticsValueViewModel> Hours { get; set; }
    }
}
