namespace HRM.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Core.Constants;
    using GO.UserControl.Client.Core;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;
    using HRM.Services;
    using HRM.ViewModels.Dashboard;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController(
        IUserControlSettings userControlSettings,
        IUserControlSession userControlSession,
        IDashboardService dashboardService,
        IUserService userService) : BaseController(userControlSession)
    {
        public async Task<IActionResult> Index()
        {
            var user = await userService.GetByIdAsync(userControlSession.UserId);
            var viewModel = new DashboardViewModel
            {
                SummaryRequests = dashboardService.GetMySpace(EmployeeSession.EmpId, DateTimeOffset.Now.DateTime)
            };
            var isVerifierApprover = userControlSession.HasPermissionOnFunction(FunctionKey.VerifyRequest, Policy.Host)
                || userControlSession.HasPermissionOnFunction(FunctionKey.ApproveRequest, Policy.Host);
            ViewBag.SiteId = userControlSettings.SiteId;
            ViewBag.IsResetPassword = user.IsResetPassword;
            ViewBag.IsVerifierApprover = isVerifierApprover;
            if (isVerifierApprover)
            {
                viewModel.MemberRequests = await dashboardService.GetMemberRequestAsync(EmployeeSession.EmpId, DateTimeOffset.Now.DateTime);
            }

            return View(viewModel);
        }
    }
}