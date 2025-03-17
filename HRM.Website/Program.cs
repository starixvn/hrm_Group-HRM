using System.IO;
using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry.RavenClient;
using GO.UserControl.Client.Core;
using GO.UserControl.Client.Core.Session;
using HRM.Website;
using HRM.Website.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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
    });

services.AddFanexLogSentry(configuration);
services.AddFanexSql(configuration);
services.AddCookieAuthentication(configuration);
services.AddRedis(configuration);
services.AddRedisSession(configuration);
services.AddDIServices(configuration);

var app = builder.Build();

UserControlClientManager
    .UseSettings(app.Services.GetService<IUserControlSettings>())
    .UseSession(app.Services.GetService<IUserControlSession>());

if (!app.Environment.IsDevelopment())
{
    app.UseMiddleware<ErrorHandler>();
    app.UseHsts();
}

app.UseHttpsRedirection();
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

await app.RunAsync();