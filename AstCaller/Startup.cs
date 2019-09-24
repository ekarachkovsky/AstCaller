using System;
using AstCaller.Classes;
using AstCaller.DataLayer;
using AstCaller.DataLayer.Implementations;
using AstCaller.DataLayer.Stores;
using AstCaller.Models;
using AstCaller.Services;
using AstCaller.Services.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AstCaller
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            var configWriter = new ConfigurationWriter(env);

            new DatabaseGenerator(configuration, configWriter).Run();
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

        private void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ISqlConnectionProvider, MySqlConnectionProvider>()
                .AddTransient<IUserRepository, UserRepository>()
                .AddTransient<ICampaignRepository,CampaignRepository>()
                .AddTransient<ICampaignAbonentRepository,CampaignAbonentRepository>()
                .AddTransient<IAbonentsFileService,AbonentsFileService>()
                .AddTransient<IUserProvider,UserProvider>()
                .AddSingleton<IUserStore<UserModel>, UserStore>()
                .AddSingleton<UserRoleStore>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                /*.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConsole();
                    loggingBuilder.AddDebug();
                })*/;
        }
    }
}
