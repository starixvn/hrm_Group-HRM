namespace Fanex.NexHR.Site
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Net;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions.Web;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.NexHR.Core.Providers;
    using GO.UserControl.Client.AspNetMvc;
    using GO.UserControl.Client.AspNetMvc.Session;
    using GO.UserControl.Client.Core;
    using SharpRaven;

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();

            LogManager
                .SetDefaultLogCategory(ConfigurationManager.AppSettings["Sentry:Category"])
                .Use(new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(ConfigurationManager.AppSettings["Sentry:Dsn"]),
                    WriteLogTimeoutSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["Sentry:Timeout"])
                }));

            ExceptionHandler
                .UseConfig(new LoggingConfiguration())
                .GetHttpHeaderInfo()
                .GetHttpSessionInfo()
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            DbSettingProviderManager
                .StartNewSession()
                .WithSqlServer("~/App_Data/SpNames.xml")
                .Run();
            ModelMetadataProviders.Current = new FriendlyNameModelMetadataProvider();

            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            UserControlClientManager
               .UseSettings(new UserControlSettings())
               .UseSession(new UserControlSession());
        }

        [ValidateInput(false)]
        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex.GetType() == typeof(UnauthorizedAccessException))
            {
                HandleResponse(HttpStatusCode.Unauthorized, "~/Auth/Login");
            }
            else
            {
                ExceptionHandler.Handle(ex, TraceEventType.Error);

                HandleResponse(HttpStatusCode.InternalServerError, "~/Error/InternalError");
            }
        }

        private static void HandleResponse(HttpStatusCode statusCode, string redirectUrl)
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null) return;

            var requestContext = ((MvcHandler)httpContext.CurrentHandler).RequestContext;
            if (requestContext.HttpContext.Request.IsAjaxRequest())
            {
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = (int)statusCode;
                httpContext.Response.End();
            }
            else
            {
                httpContext.Response.Redirect(redirectUrl);
            }
        }
    }
}