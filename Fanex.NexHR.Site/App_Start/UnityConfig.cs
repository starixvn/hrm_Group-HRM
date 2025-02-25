namespace Fanex.NexHR.Site
{
    using System.Web.Http;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Services.Extensions;
    using Unity;
    using Unity.Mvc5;

    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.ResolveCoreDI();
            container.ResolveServiceCoreDI();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }
    }
}