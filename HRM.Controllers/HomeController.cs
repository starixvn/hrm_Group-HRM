namespace HRM.Controllers
{
    using Core.Constants;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;
    using HRM.Services;
    using HRM.ViewModels.Dashboard;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Threading.Tasks;

    public class HomeController(
        IUserControlSession userControlSession,
        IDashboardService dashboardService,
        IUserService userService,
        IConfiguration configuration) : BaseController(userControlSession)
    {
        private readonly IUserControlSession userControlSession = userControlSession;

        public async Task<IActionResult> Index()
        {
            var user = await userService.GetByIdAsync(userControlSession.UserId);
            var viewModel = new DashboardViewModel
            {
                SummaryRequests = dashboardService.GetMySpace(EmployeeSession.EmpId, DateTimeOffset.Now.DateTime)
            };
            var isVerifierApprover = userControlSession.HasPermissionOnFunction(FunctionKey.VerifyRequest, Policy.Host)
                || userControlSession.HasPermissionOnFunction(FunctionKey.ApproveRequest, Policy.Host);
            ViewBag.SiteId = int.Parse(configuration.GetSection("GO.UserControl.Client:SiteId").Value);
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