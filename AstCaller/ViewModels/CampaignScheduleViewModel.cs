using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AstCaller.ViewModels
{
    public class CampaignScheduleViewModel
    {
        public int? Id { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime DateStart { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime DateEnd { get; set; }

        public string TimeEnd { get; set; }

        public string TimeStart { get; set; }

        public int DaysOfWeek { get; set; }
    }
}
