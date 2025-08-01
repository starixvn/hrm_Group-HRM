using System.Net.Http;
using System.Threading;
using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry.RavenClient;
using GO.UserControl.Client.Core;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Website;
using HRM.Website.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
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

services.AddHttpContextAccessor();
services.AddHttpClient();
services.AddHttpClient("insecure-client")
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
    });
services.AddFanexLogSentry(configuration);
services.AddFanexSql(configuration);
services.AddCookieAuthentication(configuration);
services.AddRedis(configuration);
services.AddRedisSession(configuration);
services.AddDIServices(configuration);

var app = builder.Build();

UserControlClient
    .UseSession(app.Services.GetRequiredService<IUserControlSession>(), configuration);
DateTimeExtensions.Configure(app.Services.GetRequiredService<IUserControlSession>());

if (!app.Environment.IsDevelopment())
{
    app.UseMiddleware<ErrorHandler>();
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), branch =>
{
    branch.UseMiddleware<ApiKeyMiddleware>();
});
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

ThreadPool.SetMinThreads(100, 100);

await app.RunAsync();