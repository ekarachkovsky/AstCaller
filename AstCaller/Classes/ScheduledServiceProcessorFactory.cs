using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstCaller.Models;
using AstCaller.Models.Domain;
using AstCaller.Services;
using AstCaller.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AstCaller.Classes
{
    public class ScheduledServiceProcessorFactory : IScheduledServiceProcessorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ScheduledServiceProcessorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IScheduledProcessorService Build(ScheduleTaskModel schedule, MainContext context)
        {
            return new ScheduledProcessorService(_serviceProvider.GetService(typeof(ILogger<ScheduledProcessorService>)) as ILogger<ScheduledProcessorService>,
                context, 
                _serviceProvider.GetService(typeof(IConfiguration)) as IConfiguration,
                schedule);
        }
    }
}
