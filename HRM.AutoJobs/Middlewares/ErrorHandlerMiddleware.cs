namespace HRM.AutoJobs.Middlewares;

using Fanex.Logging;
using Microsoft.AspNetCore.Http;

public class ErrorHandlerMiddleware(RequestDelegate next, ILogger logger);