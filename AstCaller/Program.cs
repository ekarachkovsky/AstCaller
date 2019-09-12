using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

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

                    if (!context.HostingEnvironment.IsDevelopment())
                    {
                        options.Listen(System.Net.IPAddress.Loopback, config.GetValue<int>("Host:Port"));
                    }
                })
                .UseStartup<Startup>();

            return webHost;
        }
    }
}
