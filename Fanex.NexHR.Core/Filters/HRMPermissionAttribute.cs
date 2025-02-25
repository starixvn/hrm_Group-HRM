namespace Fanex.NexHR.Core.Filters
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Web.Mvc;
    using System.Web.Routing;
    using GO.UserControl.Client.Core;
    using GO.UserControl.Client.Core.Enumerables;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class HRMPermissionAttribute : FilterAttribute, IAuthorizationFilter
    {
        public HRMPermissionAttribute(Policy policy, params string[] functionKeys)
        {
            Policy = policy;
            FunctionKeys = functionKeys;
        }

        public string[] FunctionKeys { get; set; }

        public Policy Policy { get; set; }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var session = UserControlClientManager.UserControlSession;
            if (!FunctionKeys.Select(functionKey => session.HasPermissionOnFunction(functionKey, Policy)).Any(x => x))
            {
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    filterContext.Result = new JsonResult { Data = (int)HttpStatusCode.Forbidden, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
        }
    }
}