namespace HRM.Controllers;

using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

[HrmAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class BaseController(IUserControlSession userControlSession) : Controller
{
    public EmployeeSession EmployeeSession => userControlSession.Get<EmployeeSession>(SessionKey.Employee) ?? new EmployeeSession();
    public int CurrentCompanyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
}