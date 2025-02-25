namespace HRM.AutoJobs
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using Fanex.Data.Repository;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using HRM.AutoJobs.Constants;
    using HRM.AutoJobs.Middlewares;
    using HRM.AutoJobs.Models;
    using HRM.AutoJobs.Services;
    using HRM.AutoJobs.Workers;
    using HRM.Letstalk.Core.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Sentry;

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
            services.AddFanexSql(Configuration);

            LogManager
                .SetDefaultLogCategory(Configuration.GetSection("SentrySettings:LogCategory").Value)
                .Use(new List<ILogEngine>
                {
                    new SentryLogging(new SentryEngineOptions
                    {
                        Dsn = new Dsn(Configuration.GetSection("SentrySettings:Dsn").Value)
                    })
                });

            ExceptionHandler
                .UseConfig(new LoggingConfiguration())
                .GetHttpHeaderInfo()
                .GetHttpSessionInfo()
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            var proxySettings = Configuration.GetSection("ProxySettings").Get<ProxySettings>();
            services.AddHttpClient(NamedHttpClients.ProxiedClient)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    UseProxy = proxySettings.UseProxy,
                    Proxy = new WebProxy(proxySettings.Address, proxySettings.BypassOnLocal)
                });

            services.AddSingleton(Configuration);

            services.AddHttpClient();
            AddDI(services);
            AddWorker(services);

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ErrorHandlerMiddleware>();
                app.UseHsts();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void AddDI(IServiceCollection services)
        {
            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            services.AddSingleton(appSettings);

            var letstalkSettings = Configuration.GetSection("LetstalkSettings").Get<LetstalkSettings>();
            services.AddSingleton(letstalkSettings);

            var jobSettings = Configuration.GetSection("JobSettings").Get<List<JobSetting>>();
            services.AddSingleton(jobSettings);

            var userControlSettings = Configuration.GetSection("UserControlSettings").Get<UserControlSettings>();
            services.AddSingleton(userControlSettings);

            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IDynamicRepository, DynamicRepository>();

            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<IBaseMailService, BaseMailService>();
            services.AddSingleton<IHttpClientService, HttpClientService>();
            services.AddSingleton<ISendNotificationService, SendNotificationService>();
            services.AddSingleton<ILetstalkService, LetstalkService>();
            services.AddSingleton<IScheduleArrangeService, ScheduleArrangeService>();
            services.AddSingleton<IAnnualLeaveBalanceService, AnnualLeaveBalanceService>();
            services.AddSingleton<IHolidayService, HolidayService>();
            services.AddSingleton<ITimeRecordService, TimeRecordService>();
            services.AddSingleton<ISyncWorkingShiftService, SyncWorkingShiftService>();
            services.AddSingleton<IPostnatalService, PostnatalService>();

            services.AddSingleton<IGroupReportService, GroupReportService>();
        }

        private static void AddWorker(IServiceCollection services)
        {
            services.AddHostedService<ImportTimeRecordWorker>();
            services.AddHostedService<InsertHolidayInYearWorker>();

            services.AddHostedService<PublicScheduleWorker>();
            services.AddHostedService<InsertDefaultShiftWorker>();
            services.AddHostedService<InsertWeeklyDefaultShiftWorker>();

            services.AddHostedService<InsertALBalanceForNextYearWorker>();
            services.AddHostedService<InsertALBalanceForNewEmployeeWorker>();
            services.AddHostedService<RemindUsedALWorker>();

            services.AddHostedService<SyncEmployeeInfoToGroupReportWorker>();
        }
    }
}