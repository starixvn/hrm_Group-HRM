namespace HRM.Website
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Caching.Redis;
    using HRM.Core.Models;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using StackExchange.Redis;

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

            return services;
        }
    }
}