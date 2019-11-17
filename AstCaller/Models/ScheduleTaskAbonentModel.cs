using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstCaller.Models
{
    public class ScheduleTaskAbonentModel
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        public string Phone { get; set; }
    }
}
