using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Web.Security;
    using Fanex.Data;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Models;
    using HRM.Services;
    using HRM.ViewModels.Employee;
    using GO.UserControl.Client.Core;
    using GO.UserControl.Client.Core.Models.Auth;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;

    public class AuthController : Controller
    {
        private readonly IHRMAuthService hrmAuthService;
        private readonly IUserControlSettings userControlSettings;
        private readonly IUserControlSession userControlSession;
        private readonly IUserService userService;
        private readonly IAuthService authService;
        private readonly ICryptoService cryptoService;

        public AuthController(
            IHRMAuthService hrmAuthService,
            IUserControlSettings userControlSettings,
            IUserControlSession userControlSession,
            IUserService userService,
            IAuthService authService,
            ICryptoService cryptoService)
        {
            this.hrmAuthService = hrmAuthService;
            this.userControlSettings = userControlSettings;
            this.userControlSession = userControlSession;
            this.userService = userService;
            this.authService = authService;
            this.cryptoService = cryptoService;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            model.Password = Encoding.UTF8.GetString(Convert.FromBase64String(model.Password));
            var token = cryptoService.GenerateRijndaelKeyIV();
            string[] keyiv = token.Split(':');
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            var loginParam = new LoginParam
            {
                Username = cryptoService.Encrypt(model.Username, rijndaelKey, rijndaelIV),
                Password = cryptoService.Encrypt(model.Password, rijndaelKey, rijndaelIV),
                Token = token,
                IP = GetClientIP(),
                Platform = Request.Browser.Platform,
                BrowserName = Request.Browser.Browser,
                NetSessionId = Session.SessionID
            };
            var loginResult = authService.Login(loginParam);
            if (loginResult.IsSuccessfull)
            {
                EmployeeSession employee = hrmAuthService.GetByUserId(loginResult.UserId);
                if (employee == null)
                {
                    loginResult = new LoginResult { IsSuccessfull = false, ErrorMessage = "User has not been mapped." };
                    return new CustomJsonResult { Data = loginResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

                userControlSession.UserId = loginResult.UserId;
                userControlSession.SiteId = userControlSettings.SiteId;
                userControlSession.JwtToken = loginResult.JwtToken;
                userControlSession.OnUserId = loginResult.OnUserId;
                userControlSession.MenuItems = userService.GetMenuItem();
                userControlSession.Functions = userService.GetFunctionPermission(userControlSession.SiteId, userControlSession.UserId);
                userControlSession.Set(SessionKey.IsShowLeftNavigation, true);

                userControlSession.Set(SessionKey.Employee, employee);
                // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                FormsAuthentication.SetAuthCookie(employee.EmpId.ToString(), false);
                return new CustomJsonResult { Data = loginResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new CustomJsonResult { Data = loginResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            authService.Logout(userControlSession.OnUserId);
            userControlSession.ClearAll();

            // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult SSO(string userId, string onUserId, string time, string redirectUrl = null)
        {
            const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            var userIdValue = Cryptography.DecryptPassword(userId.Replace(" ", "+"));
            var onUserIdValue = Cryptography.DecryptPassword(onUserId.Replace(" ", "+"));
            var timeValue = Cryptography.DecryptPassword(time.Replace(" ", "+"));
            var currentTime = authService.SSOGetTime();
            TimeSpan expiration = DateTime.ParseExact(currentTime, DateTimeFormat, new CultureInfo("en-US")) - DateTime.ParseExact(timeValue, DateTimeFormat, new CultureInfo("en-US"));
            const byte TimeExpiration = 30;
            if (expiration.TotalSeconds <= TimeExpiration)
            {
                var ssoParam = new SSOLoginParam
                {
                    UserId = Convert.ToInt32(userIdValue),
                    CheckOnUserId = Convert.ToInt64(onUserIdValue),
                    OnUserId = 0,
                    IP = GetClientIP(),
                    Platform = Request.Browser.Platform,
                    BrowserName = Request.Browser.Browser,
                    NetSessionId = Session.SessionID
                };
                LoginResult loginResult = authService.SSOLogin(ssoParam);
                if (loginResult.IsSuccessfull)
                {
                    EmployeeSession employee = hrmAuthService.GetByUserId(Convert.ToInt32(userIdValue));
                    if (employee == null)
                    {
                        return RedirectToAction("Login");
                    }

                    userControlSession.UserId = Convert.ToInt32(userIdValue);
                    userControlSession.SiteId = userControlSettings.SiteId;
                    userControlSession.JwtToken = loginResult.JwtToken;
                    userControlSession.OnUserId = loginResult.OnUserId;
                    userControlSession.MenuItems = userService.GetMenuItem();
                    userControlSession.Functions = userService.GetFunctionPermission(userControlSession.SiteId, userControlSession.UserId);
                    userControlSession.Set(SessionKey.IsShowLeftNavigation, true);

                    userControlSession.Set(SessionKey.Employee, employee);
                    // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
                    FormsAuthentication.SetAuthCookie(employee.EmpId.ToString(), false);
                    return !string.IsNullOrWhiteSpace(redirectUrl) && Url.IsLocalUrl(redirectUrl) ? Redirect(redirectUrl) : Redirect("~/Home/Index");
                }
            }

            return RedirectToAction("Login");
        }

        private string GetClientIP()
        {
            var clientIp = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrWhiteSpace(clientIp))
            {
                var forwardedIps = clientIp.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                clientIp = forwardedIps[forwardedIps.Length - 1];
            }
            else
            {
                clientIp = Request.ServerVariables["REMOTE_ADDR"];
            }

            if (clientIp == "::1")
            {
                clientIp = "127.0.0.1";
            }

            return clientIp;
        }
    }
}