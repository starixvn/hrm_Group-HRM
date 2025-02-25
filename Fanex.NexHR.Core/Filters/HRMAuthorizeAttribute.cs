namespace Fanex.NexHR.Core.Filters
{
    using System;
    using System.Net;
    using System.Web.Mvc;
    using GO.UserControl.Client.Core;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HRMAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var session = UserControlClientManager.UserControlSession;
            if (session.UserId > 0 && session.JwtToken != null && !session.JwtToken.IsExpired) return;

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                filterContext.Result = new JsonResult { Data = (int)HttpStatusCode.Unauthorized, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}