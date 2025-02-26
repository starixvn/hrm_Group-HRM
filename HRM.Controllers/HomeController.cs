namespace HRM.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Core.Constants;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;
    using HRM.Services;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController(
        IUserControlSession userControlSession,
        IDashboardService dashboardService,
        IUserService userService) : BaseController(userControlSession)
    {
        private readonly IUserControlSession userControlSession = userControlSession;

        public async Task<IActionResult> Index()
        {
            var user = await userService.GetByIdAsync(userControlSession.UserId);
            var model = dashboardService.GetMySpace(EmployeeSession.EmpId, DateTimeOffset.Now.DateTime);
            ViewBag.IsResetPassword = user.IsResetPassword;
            ViewBag.IsVerifierApprover = userControlSession.HasPermissionOnFunction(FunctionKey.VerifyRequest, Policy.Host)
                || userControlSession.HasPermissionOnFunction(FunctionKey.ApproveRequest, Policy.Host);

            return View(model);
        }
    }
}