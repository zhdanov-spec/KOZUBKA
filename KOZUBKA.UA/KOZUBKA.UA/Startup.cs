using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ua.kozubka.context.Classes.Context;
using ua.kozubka.context.Services;
using ua.kozubka.context.Services.Filtres;
using ua.kozubka.context.Services.RecuringJobs;

namespace ua.kozubka
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }
        
        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMainServices(Configuration);
            services.AddRepositories();
            services.AddMailRepository();
            services.AddHangFireRepository(Configuration);
            services.AddBotRepository();
           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            app.UseAuthentication();

            app.UseCors();
            
            app.UseAuthorization();

            app.UseHttpContext();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


            ////Hangfire Shceduling
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangFireFilter() }
            });
            //https://crontab.cronhub.io/
            recurringJobManager.AddOrUpdate("Mail And Bot Group Every 1 minutes", () => serviceProvider.GetService<IRecuringJob>().RefreshEvery1Minutes(), "* * * * *", TimeZoneInfo.Local);
            recurringJobManager.AddOrUpdate("Every 3 Hour Jobs", () => serviceProvider.GetService<IRecuringJob>().RefreshEvery3Hours(), "0 30 */3 * * *", TimeZoneInfo.Local);
            recurringJobManager.AddOrUpdate("Every at 9.00am Jobs", () => serviceProvider.GetService<IRecuringJob>().RefreshEvery9amHours(), "0 0 9 * * *", TimeZoneInfo.Local);
            recurringJobManager.AddOrUpdate("Every 1 day of mont at 9.00am Jobs",()=>serviceProvider.GetService<IRecuringJob>().RefreshEvery1dayMonth(), "0 0 9 1 * *", TimeZoneInfo.Local);
        }
    }
}
