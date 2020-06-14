using System.ComponentModel.DataAnnotations;

namespace AstCaller.ViewModels.Reports
{
    public class ReportCallMeViewModel
    {
        public int Id { get; set; }

        [Display(Name ="Телефон")]
        public string Phone { get; set; }
    }
}
