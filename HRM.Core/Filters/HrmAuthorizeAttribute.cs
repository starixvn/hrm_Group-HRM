namespace HRM.Core.Filters
{
    using System;
    using GO.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HrmAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.RequestServices.GetRequiredService<IUserControlSession>();
            if (session.UserId <= 0 || session.JwtToken == null || session.JwtToken.IsExpired)
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
        }

        private static bool IsAjaxRequest(HttpRequest request)
            => string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
    }
}