namespace HRM.Controllers;

using GO.UserControl.Client.Core.Session;
using Core.Constants;
using Core.Filters;
using HRM.Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

[HrmAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class BaseController(IUserControlSession userControlSession) : Controller
{
    protected EmployeeSession EmployeeSession => userControlSession.Get<EmployeeSession>(SessionKey.Employee) ?? new EmployeeSession();
    protected short CurrentCompanyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);

    protected string GetDomainName(HttpContext httpContext)
    {
        var protocol = httpContext.Request.Headers["x-forwarded-proto"].ToString();
        return $"{(!string.IsNullOrWhiteSpace(protocol) ? protocol : httpContext.Request.Scheme)}://{httpContext.Request.Host.Value}";
    }
}