using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System;
    using System.Collections.Generic;
    using Core.Constants;
    using HRM.Models.Dashboard;
    using HRM.Services;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;

    public class HomeController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IDashboardService dashboardService;
        private readonly IUserService userService;

        public HomeController(
            IUserControlSession userControlSession,
            IDashboardService dashboardService,
            IUserService userService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.dashboardService = dashboardService;
            this.userService = userService;
        }

        public ActionResult Index()
        {
            var user = userService.GetById(userControlSession.UserId);
            IEnumerable<SummaryRequest> model = dashboardService.GetMySpace(EmployeeSession.EmpId, DateTimeOffset.Now.DateTime);
            ViewBag.IsResetPassword = user.IsResetPassword;
            ViewBag.IsVerifierApprover = userControlSession.HasPermissionOnFunction(FunctionKey.VerifyRequest, Policy.Host)
                || userControlSession.HasPermissionOnFunction(FunctionKey.ApproveRequest, Policy.Host);

            return View(model);
        }

        // TODO Child actions should be replaced with view components. For more details see https://docs.microsoft.com/aspnet/core/mvc/views/view-components and https://www.davepaquette.com/archive/2016/01/02/goodbye-child-actions-hello-view-components.aspx.
        [ChildActionOnly]
        public ActionResult MemberRequest()
        {
            IEnumerable<MemberRequest> model = dashboardService.GetMemberRequest(EmployeeSession.EmpId, DateTimeOffset.Now.DateTime);

            return PartialView("_MemberRequest", model);
        }
    }
}