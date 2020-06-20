using AstCaller.Models.Domain;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace AstCaller
{
    public static class Program
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
                    if (!context.HostingEnvironment.IsDevelopment())
                    {

                        var config = new ConfigurationBuilder()
                                    .SetBasePath(context.HostingEnvironment.ContentRootPath)
                                    .AddJsonFile("appsettings.json", optional: false)
                                    .Build();
                        options.Listen(System.Net.IPAddress.Loopback, config.GetValue<int>("Host:Port"));
                    }
                })
                .UseStartup<Startup>()
                .UseSerilog((hostingContext, loggerConfiguration) => 
                    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)); ;

            return webHost;
        }
    }
}
