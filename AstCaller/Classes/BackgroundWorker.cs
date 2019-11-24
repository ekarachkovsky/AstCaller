using AstCaller.Models.Domain;
using AstCaller.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IScheduledServiceProcessorFactory _serviceFactory;
        private readonly IServiceProvider _serviceProvider;

        public BackgroundWorker(ILogger<BackgroundWorker> logger,
            IScheduledServiceProcessorFactory scheduledServiceProcessorFactory,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceFactory = scheduledServiceProcessorFactory;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Initialize background worker");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    using (var context = scope.ServiceProvider.GetRequiredService<MainContext>())
                    {
                        var scheduleService = scope.ServiceProvider.GetRequiredService<IScheduleService>();
                        var schedules = await scheduleService.GetCurrentSchedulesAsync();
                        foreach (var schedule in schedules)
                        {
                            var service = _serviceFactory.Build(schedule, context);
                            await service.ExecuteAsync();
                        }

                        var callFinalizer = scope.ServiceProvider.GetRequiredService<ICallFinalizer>();
                        await callFinalizer.ExecuteAsync();
                        await callFinalizer.CleanupAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Background Worker exception");
                }

                await Task.Delay(30000, stoppingToken);
            }
        }
    }
}
