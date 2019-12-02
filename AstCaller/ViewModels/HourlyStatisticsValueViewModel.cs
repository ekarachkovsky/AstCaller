using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstCaller.ViewModels
{
    public class HourlyStatisticsValueViewModel
    {
        public int Hour { get; set; }
        public Dictionary<int,int> Counts { get; set; }
    }
}
