using System;
using System.Globalization;
using System.Linq;
using AstCaller.Classes;
using AstCaller.DataLayer;
using AstCaller.DataLayer.Implementations;
using AstCaller.DataLayer.Stores;
using AstCaller.Models;
using AstCaller.Models.Domain;
using AstCaller.Services;
using AstCaller.Services.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AstCaller
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IConfiguration), Configuration);
            RegisterServices(services);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddIdentity<UserModel, UserRoleModel>(options =>
                     {
                         options.Password.RequireDigit = false;
                         options.Password.RequiredUniqueChars = 6;
                         options.Password.RequireLowercase = false;
                         options.Password.RequireNonAlphanumeric = false;
                         options.Password.RequireUppercase = false;
                     })
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore>()
                .AddRoleStore<UserRoleStore>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var supportedCultures = new[] { new CultureInfo("ru-RU") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru-RU"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

            }
            var rootPath = Configuration.GetValue<string>("Host:RootPath");
            if (!string.IsNullOrEmpty(rootPath))
            {
                app.UsePathBase(rootPath);
                app.Use((context, next) =>
                {
                    context.Request.PathBase = new PathString(rootPath);
                    return next.Invoke();
                });
            }

            ConfigureDatabase(app);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void ConfigureDatabase(IApplicationBuilder app)
        {
            if (Configuration.GetValue<bool>("Db:FastStart"))
            {
                return;
            }
            using (var serviceScope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<MainContext>())
                {
                    context.Database.Migrate();
                    if (!context.Users.Any())
                    {
                        var service = new SeedDatabase(context);
                        service.CreateAdminUser();
                    }

                    if (!context.CallStatuses.Any())
                    {
                        var service = new SeedDatabase(context);
                        service.CreateCallStatuses();
                    }

                    if (!context.AsteriskExtensions.Any())
                    {
                        var service = new SeedDatabase(context);
                        service.CreateAsteriskExtensions();
                    }
                }
            }
        }

        private void RegisterServices(IServiceCollection services)
        {
            services
                .AddDbContext<MainContext>(options =>
                        options.UseMySql(Configuration.GetConnectionString("DefaultConnection")))
                .AddTransient<IAbonentsFileService, AbonentsFileService>()
                .AddTransient<IUserProvider, UserProvider>()
                .AddSingleton<IUserStore<UserModel>, UserStore>()
                .AddSingleton<UserRoleStore>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddTransient<IScheduleService, ScheduleService>()
                .AddTransient<IScheduledServiceProcessorFactory, ScheduledServiceProcessorFactory>()
                .AddTransient<ICallFinalizer, CallFinalizer>()
                .AddTransient<IReportingService,ReportingService>()
                .AddSingleton<Microsoft.Extensions.Hosting.IHostedService, BackgroundWorker>()
                .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
            /*.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            })*/
            ;
        }
    }
}
