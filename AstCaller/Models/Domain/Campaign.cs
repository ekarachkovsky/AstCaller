using System;

namespace AstCaller.Models.Domain
{
    public class Campaign : BaseModel
    {
        public string Name { get; set; }

        public int Status { get; set; }

        public int AbonentsCount { get; set; }

        public string AbonentsFileName { get; set; }

        public string VoiceFileName { get; set; }

        public int ModifierId { get; set; }
        
        public DateTime Modified { get; set; }
    }
}
