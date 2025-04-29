using System;
using System.Net;
using System.Threading.Tasks;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Microsoft.AspNetCore.Http;

namespace HRM.Website.Middlewares;

public class ErrorHandler(RequestDelegate next, ILogger logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (exception.GetType() == typeof(UnauthorizedAccessException))
        {
            await HandleUnauthorizedAsync(context, exception);
        }
        else
        {
            var logMessage = new LogMessage(exception.Message)
                .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                .ToString();
            await logger.ErrorAsync(logMessage);

            if (IsAjaxRequest(context.Request))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(exception.Message);
            }
            else
            {
                context.Response.Redirect("/Error/InternalError");
            }
        }
    }

    private static async Task HandleUnauthorizedAsync(HttpContext context, Exception exception)
    {
        if (exception is UnauthorizedAccessException)
        {
            if (IsAjaxRequest(context.Request))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync(exception.Message);
            }
            else
            {
                context.Response.Redirect("/Auth/Login");
            }
        }
    }

    private static bool IsAjaxRequest(HttpRequest request)
        => string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
           string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
}