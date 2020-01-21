using System.ComponentModel.DataAnnotations;

namespace AstCaller.ViewModels.Reports
{
    public class ReportFailedCallsViewModel
    {
        [Display(Name ="Кол-во попыток")]
        public int Attempts { get; set; }

        [Display(Name ="Статус")]
        public string Status { get; set; }

        [Display(Name ="Телефон")]
        public string Phone { get; set; }
        public int Id { get; set; }
    }
}
