using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UsingHangfire
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(c =>
            {
                c.UseSqlServerStorage("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=HangfireDB;Data Source=.");
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            app.UseMvcWithDefaultRoute();

            //Recurring jobs fire many times on the specified CRON schedule.
            RecurringJob.AddOrUpdate<SomeClass>(x => x.SomeMethod(), Cron.Daily, TimeZoneInfo.Local);

            RecurringJob.AddOrUpdate<JobClass>(x => x.Execute(), Cron.Daily(13,0),TimeZoneInfo.Local);
        }
    }
}
