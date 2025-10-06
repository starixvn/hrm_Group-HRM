using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Fanex.Data;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.ViewModels.UI;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Website.Views.Shared.Components.Header;

public class HeaderViewComponent(
    IUserService userService, IUserControlSession userControlSession) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.UserId.ToString()));
        var onUserId = HttpUtility.UrlEncode(Cryptography.EncryptPassword(userControlSession.OnUserId.ToString()));
        var userSites = await userService.GetSiteAsync(userControlSession.UserId);
        var currentSiteId = userControlSession.SiteId;

        var filteredUserSites = userSites.Where(site => site.SiteId != currentSiteId);

        foreach (var userSite in filteredUserSites)
        {
            userSite.SiteUrl = $"{userSite.SiteUrl}/auth/sso?userId={userId}&onUserId={onUserId}";
        }

        var viewModel = new HeaderViewModel
        {
            Employee = userControlSession.Get<EmployeeSession>(SessionKey.Employee) ?? new EmployeeSession(),
            IsShowSearchBox = userControlSession.HasPermissionOnFunction(FunctionKey.ContactList, Policy.Read),
            UserSites = filteredUserSites
        };
        ViewBag.IsShowLeftNavigation = userControlSession.Get<bool>(SessionKey.IsShowLeftNavigation);

        return View("Header", viewModel);
    }
}