using System.Threading;
using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry.RavenClient;
using GO.UserControl.Client.Core;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Website;
using HRM.Website.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
        options.SerializerSettings.DateFormatString = "dd MMM, yyyy HH:mm:ss";
    });

services.AddHttpClient();
services.AddFanexLogSentry(configuration);
services.AddFanexSql(configuration);
services.AddCookieAuthentication(configuration);
services.AddRedis(configuration);
services.AddRedisSession(configuration);
services.AddDIServices(configuration);

var app = builder.Build();

UserControlClientManager
    .UseSettings(app.Services.GetRequiredService<IUserControlSettings>())
    .UseSession(app.Services.GetRequiredService<IUserControlSession>());
DateTimeExtensions.Configure(app.Services.GetRequiredService<IUserControlSession>());

if (!app.Environment.IsDevelopment())
{
    app.UseMiddleware<ErrorHandler>();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

ThreadPool.SetMinThreads(100, 100);

await app.RunAsync();