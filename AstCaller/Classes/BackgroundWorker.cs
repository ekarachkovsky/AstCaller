using AstCaller.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AstCaller.Classes
{
    public class BackgroundWorker : BackgroundService
    {
        private readonly ILogger<BackgroundWorker> _logger;
        private readonly IScheduleService _scheduleService;
        private readonly IScheduledServiceProcessorFactory _serviceFactory;

        public BackgroundWorker(ILogger<BackgroundWorker> logger,
            IScheduleService scheduleService,
            IScheduledServiceProcessorFactory scheduledServiceProcessorFactory)
        {
            _logger = logger;
            _scheduleService = scheduleService;
            _serviceFactory = scheduledServiceProcessorFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Initialize background worker");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var schedules = await _scheduleService.GetCurrentSchedulesAsync();
                    foreach (var schedule in schedules)
                    {
                        var service = _serviceFactory.Build(schedule);
                        await service.ExecuteAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,"Background Worker exception");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
