using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Fanex.Data.SqlServer;
using HRM.Website;
using Fanex.Logging.Sentry;

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

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapDefaultControllerRoute();

await app.RunAsync();