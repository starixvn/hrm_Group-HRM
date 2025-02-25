namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.UI;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Models;
    using GO.UserControl.Client.Core.Session;

    public class UIController : BaseController
    {
        private readonly IMessageService messageService;
        private readonly IUserControlSession userControlSession;

        public UIController(
            IUserControlSession userControlSession,
            IMessageService messageService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.messageService = messageService;
        }

        [ChildActionOnly]
        public ActionResult GlobalScript()
        {
            ViewBag.IsShowLeftNavigation = userControlSession.Get<bool>(SessionKey.IsShowLeftNavigation);
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult Header()
        {
            var viewModel = new HeaderViewModel
            {
                SiteId = ConfigSettings.SiteId,
                Employee = EmployeeSession,
                IsShowSearchBox = userControlSession.HasPermissionOnFunction(FunctionKey.ContactList, Policy.Read)
            };

            return PartialView("_Header", viewModel);
        }

        [ChildActionOnly]
        public PartialViewResult NavBar()
        {
            IEnumerable<MenuItem> menus = userControlSession.MenuItems;
            var routeData = ControllerContext.ParentActionViewContext.RouteData;
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];
            var link = string.Format("/{0}/{1}", controller, action);
            var selectedMenu = menus.FirstOrDefault(i => !string.IsNullOrEmpty(i.URL) && i.URL.Equals(link, StringComparison.OrdinalIgnoreCase));
            var unreadMessageCount = messageService.GetUnreadMessageOfEmployee(EmployeeSession.EmpId).Count;

            ViewBag.SelectedMenu = selectedMenu;
            ViewBag.UnreadMessage = unreadMessageCount;
            return PartialView("_NavBar", menus);
        }
    }
}