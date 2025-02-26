namespace HRM.Core.Filters
{
    using System;
    using System.Linq;
    using System.Net;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class HrmPermissionAttribute(Policy policy, params string[] functionKeys) : Attribute, IAuthorizationFilter
    {
        public string[] FunctionKeys { get; set; } = functionKeys;
        public Policy Policy { get; set; } = policy;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.RequestServices.GetRequiredService<IUserControlSession>();
            if (!FunctionKeys.Select(functionKey => session.HasPermissionOnFunction(functionKey, Policy)).Any())
            {
                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    context.Result = new UnauthorizedResult();
                }
                else
                {
                    context.Result = new RedirectToActionResult("logout", "auth", null);
                }
            }
            else
            {
                CheckUserPermission(context, session);
            }
        }

        private void CheckUserPermission(AuthorizationFilterContext context, IUserControlSession session)
        {
            var hasPermission = false;

            foreach (var functionKey in FunctionKeys)
            {
                if (session.HasPermissionOnFunction(functionKey, Policy))
                {
                    hasPermission = true;
                }
            }

            if (!hasPermission)
            {
                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                }
                else
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
                }
            }
        }

        private static bool IsAjaxRequest(HttpRequest request)
           => string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
              string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
    }
}