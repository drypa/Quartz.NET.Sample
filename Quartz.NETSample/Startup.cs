using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Quartz.NETSample
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
            services.AddMvc();
            services.AddBackupScheduling();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseBackupScheduling();
        }
    }

    public static class ScheduleBackupTasksExtensions
    {
        public static void UseBackupScheduling(this IApplicationBuilder app)
        {
            var manager = app.ApplicationServices.GetService<ScheduleManager>();
            manager.TriggerScheduler().Wait();
        }
    }

    public static class ScheduleBackupCollectionExtensions
    {
        public static void AddBackupScheduling(
            this IServiceCollection services)
        {
            services.AddSingleton<ScheduleManager>();
        }
    }
}