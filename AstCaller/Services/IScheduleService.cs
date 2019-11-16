using AstCaller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstCaller.Services
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleTaskModel>> GetCurrentSchedulesAsync();
    }
}
