using System.Linq;
using BIO.WebAPI.Middlewares;
using BIO.WebAPI.Services;
using Fanex.Data;
using Fanex.Data.Postgres;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
services.AddControllers();
services.AddFanexLogSentry(configuration);

var connections = configuration.GetSection("ConnectionStrings")
    .GetChildren()
    .AsEnumerable()
    .ToDictionary(
        x => x.GetValue<string>("Name"),
        y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName"))
    );

if (connections.First().Value.ProviderName == "Postgres.Data.PostgresClient")
{
    services.AddFanexPostgres(configuration);
}
else
{
    services.AddFanexSql(configuration);
}

services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();
services.AddSingleton<IDynamicRepository, DynamicRepository>();
services.AddSingleton<IBaseService, BaseService>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseMiddleware<ErrorHandler>();
}

app.UseMiddleware<ApiKeyMiddleware>();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();