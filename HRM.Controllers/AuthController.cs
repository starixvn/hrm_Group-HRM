namespace HRM.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using GO.UserControl.Client.Core;
    using GO.UserControl.Client.Core.Models.Auth;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;
    using HRM.Core.Constants;
    using HRM.Services;
    using HRM.ViewModels.Employee;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class AuthController(
        IHRMAuthService hrmAuthService,
        IUserControlSettings userControlSettings,
        IUserControlSession userControlSession,
        IUserService userService,
        IAuthService authService,
        ICryptoService cryptoService) : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            model.Password = Encoding.UTF8.GetString(Convert.FromBase64String(model.Password));
            var token = cryptoService.GenerateRijndaelKeyIV();
            var keyiv = token.Split(':');
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            var loginParam = new LoginParam
            {
                Username = cryptoService.Encrypt(model.Username, rijndaelKey, rijndaelIV),
                Password = cryptoService.Encrypt(model.Password, rijndaelKey, rijndaelIV),
                Token = token,
                IP = GetClientIP(HttpContext),
                Platform = HttpContext.Request?.Headers["User-Agent"].ToString(),
                BrowserName = HttpContext.Request?.Headers["User-Agent"].ToString(),
                NetSessionId = HttpContext.Session.Id
            };
            var loginResult = await authService.LoginAsync(loginParam);
            if (loginResult.IsSuccessfull)
            {
                var employee = hrmAuthService.GetByUserId(loginResult.UserId);
                if (employee == null)
                {
                    loginResult = new LoginResult { IsSuccessfull = false, ErrorMessage = "User has not been mapped." };
                    return new JsonResult(loginResult);
                }

                userControlSession.UserId = loginResult.UserId;
                userControlSession.SiteId = userControlSettings.SiteId;
                userControlSession.JwtToken = loginResult.JwtToken;
                userControlSession.OnUserId = loginResult.OnUserId;
                userControlSession.MenuItems = await userService.GetMenuItemAsync();
                userControlSession.Functions = await userService.GetFunctionPermissionAsync(userControlSession.SiteId, userControlSession.UserId);
                userControlSession.Set(SessionKey.IsShowLeftNavigation, true);

                userControlSession.Set(SessionKey.Employee, employee);

                await DoLoginAsync(loginResult.UserId);
            }

            return new JsonResult(loginResult);
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

        private static string GetClientIP(HttpContext httpContext)
        {
            var clientIp = httpContext.Request.Headers["x-forwarded-for"].ToString();
            if (string.IsNullOrEmpty(clientIp))
            {
                clientIp = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
            else
            {
                var forwardedIps = clientIp.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                clientIp = forwardedIps[0];
            }

            return clientIp;
        }

    }
}