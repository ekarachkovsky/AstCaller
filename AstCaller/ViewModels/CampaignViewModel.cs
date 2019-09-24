using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AstCaller.ViewModels
{
    public class CampaignViewModel
    {
        public enum CampaignStatuses
        {
            /// <summary>
            /// Campaign was not run
            /// </summary>
            Created,

            /// <summary>
            /// Campaign is not running now
            /// </summary>
            Stopped,

            /// <summary>
            /// Campaign is running now
            /// </summary>
            Running,

            /// <summary>
            /// Campaign was cancelled
            /// </summary>
            Cancelled
        }

        [HiddenInput]
        public int? Id { get; set; }

        [Display(Name="Наименование")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Статус")]
        public CampaignStatuses Status { get; set; }

        [Display(Name = "Количество абонентов")]
        public int AbonentsTotal { get; set; }

        public int AbonentsProcessed { get; set; }

        public string AbonentsFileName { get; set; }

        public string VoiceFileName { get; set; }

        [Display(Name = "Список абонентов")]
        public IFormFile AbonentsFile { get; set; }

        [Display(Name = "Голосовая запись")]
        public IFormFile VoiceFile { get; set; }
    }
}
