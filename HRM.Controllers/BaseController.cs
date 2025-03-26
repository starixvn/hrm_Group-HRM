namespace HRM.Controllers;

using Core.Constants;
using Core.Filters;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[HrmAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class BaseController(IUserControlSession session) : Controller
{
    protected EmployeeSession EmployeeSession => session.Get<EmployeeSession>(SessionKey.Employee) ?? new EmployeeSession();
    protected short CurrentCompanyId = session.Get<short>(SessionKey.CurrentCompanyId);

    protected string GetDomainName(HttpContext httpContext)
    {
        var protocol = httpContext.Request.Headers["x-forwarded-proto"].ToString();
        return $"{(!string.IsNullOrWhiteSpace(protocol) ? protocol : httpContext.Request.Scheme)}://{httpContext.Request.Host.Value}";
    }
}