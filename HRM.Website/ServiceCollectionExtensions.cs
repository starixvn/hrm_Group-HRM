namespace HRM.Website
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Caching.Redis;
    using Fanex.Data.Repository;
    using GO.UserControl.Client.AspNetCore.Session;
    using GO.UserControl.Client.AspNetCore;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;
    using GO.UserControl.Client.Core;
    using HRM.Core.Models;
    using HRM.Core.Repositories;
    using HRM.Core.Services;
    using HRM.MailDeliver;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using StackExchange.Redis;
    using Fanex.Logging;
    using HRM.Services;
    using HRM.Repositories;
    using HRM.Services.Implement;
    using HRM.Letstalk.Core.Services;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCookieAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = configuration.GetSection("AuthSettings:LoginPath").Value;
                    options.LogoutPath = configuration.GetSection("AuthSettings:LogoutPath").Value;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(configuration.GetSection("AuthSettings:CookieTimeOut").Value));
                    options.Cookie = new CookieBuilder
                    {
                        Name = "HRM.Website.Cookies",
                        SecurePolicy = CookieSecurePolicy.SameAsRequest
                    };
                    options.Events.OnSigningOut = x =>
                    {
                        x.Response.Redirect(options.LoginPath);
                        return Task.CompletedTask;
                    };
                });

            return services;
        }

        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRedisCache(configuration.GetSection("fanex.caching.redis"));

            var redisConfig = configuration.GetSection("AuthSettings:RedisConfig").Value;
            var redisMultiplexer = ConnectionMultiplexer.Connect(redisConfig);

            services.AddDataProtection().PersistKeysToStackExchangeRedis(redisMultiplexer, "HRM.Website-ProtectionKeys");

            services.AddSingleton(redisMultiplexer);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfig;
                options.InstanceName = "HRM.Website.Session-";
            });

            return services;
        }

        public static IServiceCollection AddRedisSession(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(int.Parse(configuration.GetSection("AuthSettings:SessionTimeOut").Value));
                options.Cookie.Name = "HRM.Website.Session";
            });

            return services;
        }

        public static IServiceCollection AddDIServices(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
            services.AddSingleton(appSettings);

            var letstalkSettings = configuration.GetSection("LetstalkSettings").Get<LetstalkSettings>();
            services.AddSingleton(letstalkSettings);

            var syncWorkingShiftSettings = configuration.GetSection("SyncWorkingShiftSettings").Get<SyncWorkingShiftSettings>();
            services.AddSingleton(syncWorkingShiftSettings);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUserControlSettings>(new UserControlSettings(configuration));
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IDynamicRepository, DynamicRepository>();

            services.AddSingleton<IUserControlSession, UserControlSession>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ICryptoService, CryptoService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IRoleService, RoleService>();
            services.AddSingleton<IFunctionService, FunctionService>();

            services.AddSingleton<IBaseRepository, BaseRepository>();
            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<IBaseMailService, BaseMailService>();
            services.AddSingleton<IHttpClientService, HttpClientService>();

            services.AddSingleton<IHRMAuthService, HRMAuthService>();
            services.AddSingleton<IDashboardService, DashboardService>();

            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<IAttachFileService, AttachFileService>();
            services.AddScoped<ICommonDataService, CommonDataService>();
            services.AddScoped<IRequestMailService, RequestMailService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ILetstalkService, LetstalkService>();
            services.AddScoped<ISendNotificationService, SendNotificationService>();
            services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
            services.AddScoped<ILeaveRequestService, LeaveRequestService>();
            services.AddScoped<ICheckInOutRequestService, CheckInOutRequestService>();
            services.AddScoped<ICheckInOutRequestRepository, CheckInOutRequestRepository>();
            services.AddScoped<ICheckInOutRequestService, CheckInOutRequestService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<ILateInEarlyOutRequestService, LateInEarlyOutRequestService>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<ILateInEarlyOutRequestRepository, LateInEarlyOutRequestRepository>();
            services.AddScoped<IChangeShiftRequestService, ChangeShiftRequestService>();
            services.AddScoped<IChangeShiftRequestRepository, ChangeShiftRequestRepository>();
            services.AddScoped<IKeepInViewRequestService, KeepInViewRequestService>();
            services.AddScoped<IKeepInViewRequestRepository, KeepInViewRequestRepository>();

            return services;
        }
    }
}