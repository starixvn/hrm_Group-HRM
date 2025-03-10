namespace HRM.Controllers;

using System;
using System.Threading.Tasks;
using Core.Constants;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Services;
using HRM.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;

public class HomeController(
    IUserControlSession userControlSession,
    IDashboardService dashboardService,
    IUserService userService) : BaseController(userControlSession)
{
    public async Task<IActionResult> Index()
    {
        var today = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        var user = await userService.GetByIdAsync(userControlSession.UserId);
        var viewModel = new DashboardViewModel
        {
            SummaryRequests = await dashboardService.GetMySpaceAsync(EmployeeSession.EmpId, today)
        };
        var isVerifierApprover = userControlSession.HasPermissionOnFunction(FunctionKey.VerifyRequest, Policy.Host)
                                 || userControlSession.HasPermissionOnFunction(FunctionKey.ApproveRequest, Policy.Host);
        ViewBag.IsResetPassword = user.IsResetPassword;
        ViewBag.IsVerifierApprover = isVerifierApprover;
        if (isVerifierApprover)
        {
            viewModel.MemberRequests = await dashboardService.GetMemberRequestAsync(EmployeeSession.EmpId, today);
        }

        return View(viewModel);
    }
}