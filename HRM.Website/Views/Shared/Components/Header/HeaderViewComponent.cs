using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.ViewModels.UI;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Website.Views.Shared.Components.Header;

public class HeaderViewComponent(IUserControlSession userControlSession) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var viewModel = new HeaderViewModel
        {
            Employee = userControlSession.Get<EmployeeSession>(SessionKey.Employee),
            IsShowSearchBox = userControlSession.HasPermissionOnFunction(FunctionKey.ContactList, Policy.Read)
        };
        ViewBag.IsShowLeftNavigation = userControlSession.Get<bool>(SessionKey.IsShowLeftNavigation);

        return View("Header", viewModel);
    }
}