using HRM.Services.Interfaces;

namespace HRM.Website.Views.Shared.Components.NavBar
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.UserControl.Client.Core.Session;
    using HRM.Core.Constants;
    using HRM.Core.Models;
    using HRM.Services;
    using Microsoft.AspNetCore.Mvc;

    public class NavBarViewComponent(
        IUserControlSession userControlSession,
        IMessageService messageService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menus = userControlSession.MenuItems;
            var controller = RouteData.Values["controller"];
            var action = RouteData.Values["action"];
            var link = $"/{controller}/{action}";
            var selectedMenu = menus.FirstOrDefault(i => !string.IsNullOrEmpty(i.URL) && i.URL.Equals(link, StringComparison.OrdinalIgnoreCase));

            var employee = userControlSession.Get<EmployeeSession>(SessionKey.Employee);
            var unreadMessageCount = (await messageService.GetUnreadMessageOfEmployeeAsync(employee.EmpId)).Count();

            ViewBag.SelectedMenu = selectedMenu;
            ViewBag.UnreadMessage = unreadMessageCount;

            return View("NavBar", menus);
        }
    }
}