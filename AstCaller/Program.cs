using AstCaller.Models.Domain;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace AstCaller
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var webHost = WebHost.CreateDefaultBuilder(args)
                .ConfigureKestrel((context, options) =>
                {
                    var config = new ConfigurationBuilder()
                                .SetBasePath(context.HostingEnvironment.ContentRootPath)
                                .AddJsonFile("appsettings.json", optional: false)
                                .Build();

                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .ReadFrom.Configuration(config)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.File("logs/system_.log", rollingInterval: RollingInterval.Day, buffered: true)
                        .CreateLogger();

                    if (!context.HostingEnvironment.IsDevelopment())
                    {
                        options.Listen(System.Net.IPAddress.Loopback, config.GetValue<int>("Host:Port"));
                    }
                })
                .UseStartup<Startup>()
                .UseSerilog();

            return webHost;
        }
    }
}
