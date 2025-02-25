using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using Fanex.Common;
using Fanex.NexHR.Core;
using Fanex.NexHR.Services.Extensions;
using Hangfire;
using Microsoft.Owin;
using Owin;
using Unity;

[assembly: OwinStartup(typeof(Fanex.NexHR.Site.Startup))]

namespace Fanex.NexHR.Site
{
    public static class Startup
    {
        public static void Configuration(IAppBuilder app)
        {
            var connectionString = Cryptography.DeCryptConnectionString(WebConfigurationManager.ConnectionStrings["nexhrx"].ConnectionString);
            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);

            var unityContainer = new UnityContainer();
            GlobalConfiguration.Configuration.UseActivator(new UnityJobActivator(unityContainer));

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorization() }
            });
            app.UseHangfireServer();

            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidationCallback;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            => sslPolicyErrors == SslPolicyErrors.None;
    }

    public class UnityJobActivator : JobActivator
    {
        private readonly IUnityContainer unityContainer;

        public UnityJobActivator(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.unityContainer.ResolveCoreDI();
            this.unityContainer.ResolveServiceCoreDI();
        }

        public override object ActivateJob(Type jobType)
        {
            return unityContainer.Resolve(jobType);
        }
    }
}