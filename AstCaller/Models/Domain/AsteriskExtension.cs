using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AstCaller.Models.Domain
{
    public class AsteriskExtension
    {
        [Key]
        public string Extension { get; set; }

        public string Title { get; set; }

        public string ExtensionCode { get; set; }

        public int ModifierId { get; set; }

        public User Modifier { get; set; }

        public IEnumerable<Campaign> Campaigns { get; set; }
        public bool Disabled { get; set; }
    }
}
