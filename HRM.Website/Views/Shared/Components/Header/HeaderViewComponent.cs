using System.Collections.Generic;
using System.Web;
using Fanex.Data;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.ViewModels.UI;
using HRM.ViewModels.UserSite;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Website.Views.Shared.Components.Header;

public class HeaderViewComponent(IUserControlSession userControlSession) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var userId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.UserId.ToString()));
        var onUserId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.OnUserId.ToString()));

        var userSites = new List<UserSite>
        {
            new() {
                Username = "test",
                SiteName = "SMS HRM",
                SSOUrl = "https://hrm-sms.nexdev.net/auth/sso"
            },
            new() {
                Username = "test",
                SiteName = "WGI HRM",
                SSOUrl = "https://hrm-wgi.klc.nexdev.net/auth/sso"
            },
            new() {
                Username = "test",
                SiteName = "Group HRM",
                SSOUrl = "https://group-hrm.klc.nexdev.net/auth/sso"
            },
            new() {
                Username = "test",
                SiteName = "FMS",
                SSOUrl = "https://fpp.klc.nexdev.net/auth/sso"
            },
            new() {
                Username = "test",
                SiteName = "Group EPA",
                SSOUrl = "https://group-epa.klc.nexdev.net/auth/sso"
            }
        };
        foreach (var userSite in userSites)
        {
            var username = !string.IsNullOrWhiteSpace(userSite.Username) ? userSite.Username : userSite.Code;
            username = !string.IsNullOrWhiteSpace(username) ? HttpUtility.UrlEncode(Cryptography.EncryptPassword(username)) : string.Empty;
            var siteUrl = !string.IsNullOrWhiteSpace(userSite.SSOUrl) ? userSite.SSOUrl : userSite.SiteURL;
            userSite.SiteURL = $"{siteUrl}?userId={userId}&onUserId={onUserId}&username={username}";
        }

        var viewModel = new HeaderViewModel
        {
            Employee = userControlSession.Get<EmployeeSession>(SessionKey.Employee) ?? new EmployeeSession(),
            IsShowSearchBox = userControlSession.HasPermissionOnFunction(FunctionKey.ContactList, Policy.Read),
            UserSites = userSites
        };
        ViewBag.IsShowLeftNavigation = userControlSession.Get<bool>(SessionKey.IsShowLeftNavigation);

        return View("Header", viewModel);
    }
}