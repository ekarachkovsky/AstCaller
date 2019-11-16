using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.Models;
using AstCaller.Services;

namespace AstCaller.Classes
{
    public interface IScheduledServiceProcessorFactory
    {
        IScheduledProcessorService Build(ScheduleTaskModel schedule);
    }
}
