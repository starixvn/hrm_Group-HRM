namespace HRM.Controllers;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.Constants;
using Criteria.CompanySite;
using Criteria.SystemSetting;
using GO.UserControl.Client.Core;
using GO.UserControl.Client.Core.Models.Auth;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using ViewModels.Employee;
using Services.Interfaces;

public class AuthController(
    IHRMAuthService hrmAuthService,
    IUserControlSettings userControlSettings,
    IUserControlSession userControlSession,
    IUserService userService,
    IAuthService authService,
    ICryptoService cryptoService,
    IBaseService baseService,
    ICompanyService companyService) : Controller
{
    public async Task<IActionResult> Login()
    {
        var companySite = await companyService.GetCompanySiteByDomainAsync();
        if (companySite == null)
        {
            return RedirectToAction("AccessDenied", "Error");
        }

        userControlSettings.SiteId = companySite.SiteId;
        userControlSettings.AccessToken = companySite.AccessToken;

        userControlSession.Set(SessionKey.CurrentCompanyId, companySite.CompanyId);

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        model.Password = Encoding.UTF8.GetString(Convert.FromBase64String(model.Password));
        var token = cryptoService.GenerateRijndaelKeyIV();
        var keyIv = token.Split(':');
        var rijndaelKey = keyIv[0];
        var rijndaelIv = keyIv[1];
        var loginParam = new LoginParam
        {
            Username = cryptoService.Encrypt(model.Username, rijndaelKey, rijndaelIv),
            Password = cryptoService.Encrypt(model.Password, rijndaelKey, rijndaelIv),
            Token = token,
            IP = GetClientIp(HttpContext),
            Platform = HttpContext.Request.Headers["User-Agent"].ToString(),
            BrowserName = HttpContext.Request.Headers["User-Agent"].ToString(),
            NetSessionId = HttpContext.Session.Id
        };
        var loginResult = await authService.LoginAsync(loginParam);
        if (loginResult.IsSuccessfull)
        {
            var employee = await hrmAuthService.GetByUserIdAsync(loginResult.UserId);
            if (employee == null)
            {
                loginResult = new LoginResult { IsSuccessfull = false, ErrorMessage = "User has not been mapped." };
                return Json(loginResult);
            }

            userControlSession.UserId = loginResult.UserId;
            userControlSession.SiteId = userControlSettings.SiteId;
            userControlSession.JwtToken = loginResult.JwtToken;
            userControlSession.OnUserId = loginResult.OnUserId;
            userControlSession.MenuItems = await userService.GetMenuItemAsync();
            userControlSession.Functions = await userService.GetFunctionPermissionAsync(userControlSession.SiteId, userControlSession.UserId);
            userControlSession.Set(SessionKey.IsShowLeftNavigation, true);
            userControlSession.Set(SessionKey.Employee, employee);
            userControlSession.Set(SessionKey.CurrentTimeZone, await GetClientTimeZoneAsync());

            await DoLoginAsync(loginResult.UserId);
        }

        return Json(loginResult);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync(userControlSession.OnUserId);
        userControlSession.ClearAll();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Login");
    }

    private async Task DoLoginAsync(int userId)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }

    private async Task<short> GetClientTimeZoneAsync()
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetSystemConfigsByKeyCriteria
        {
            CompanyId = companyId,
            Key = SystemConfigKey.TimeZone
        };

        return await baseService.GetDataEntityAsync<short>(criteria);
    }

    private static string GetClientIp(HttpContext httpContext)
    {
        var clientIp = httpContext.Request.Headers["x-forwarded-for"].ToString();
        if (string.IsNullOrEmpty(clientIp))
        {
            clientIp = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
        else
        {
            var forwardedIps = clientIp.Split([','], StringSplitOptions.RemoveEmptyEntries);

            clientIp = forwardedIps[0];
        }

        return clientIp;
    }
}