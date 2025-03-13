using System;
using System.IO;
using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry;
using GO.UserControl.Client.Core;
using GO.UserControl.Client.Core.Session;
using HRM.Website;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Info("Application start.");

    var builder = WebApplication.CreateBuilder(args);

    var services = builder.Services;
    var configuration = builder.Configuration;

    // Add services to the container.
    services.AddControllersWithViews()
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
        });

    services.AddFanexLogSentry(configuration);
    services.AddFanexSql(configuration);
    services.AddCookieAuthentication(configuration);
    services.AddRedis(configuration);
    services.AddRedisSession(configuration);
    services.AddDIServices(configuration);

    services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddNLog();
    });

    var app = builder.Build();

    UserControlClientManager
        .UseSettings(app.Services.GetService<IUserControlSettings>())
        .UseSession(app.Services.GetService<IUserControlSession>());

    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    app.UseStaticFiles();
    var staticFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "file");
    if (Directory.Exists(staticFilesPath))
    {
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(staticFilesPath),
            RequestPath = "/file"
        });
    }
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSession();

    app.MapControllerRoute(
        "default",
        "{controller=Home}/{action=Index}/{id?}");

    app.Urls.Add("http://*:5000");
    await app.RunAsync();
}
catch (Exception ex)
{
    logger.Error(ex, "Application terminated unexpectedly.");
    throw;
}
finally
{
    LogManager.Shutdown();
}