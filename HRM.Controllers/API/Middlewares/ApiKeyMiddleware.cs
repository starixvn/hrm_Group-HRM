using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HRM.Controllers.API.Middlewares;

public class ApiKeyMiddleware(RequestDelegate next, IApiKeyValidator validator)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var userApiKey = context.Request.Headers["X-API-Key"];
        if (string.IsNullOrWhiteSpace(userApiKey))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync("Bad Request");
            return;
        }

        if (!validator.IsValid(userApiKey))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        await next(context);
    }
}