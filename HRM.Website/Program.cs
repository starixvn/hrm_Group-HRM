using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry;
using GO.UserControl.Client.Core;
using GO.UserControl.Client.Core.Session;
using HRM.Website;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
services.AddControllersWithViews();

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

await app.RunAsync();