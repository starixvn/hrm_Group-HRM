namespace HRM.Controllers;

using System;
using System.Threading.Tasks;
using Core.Constants;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using ViewModels.Dashboard;

public class HomeController(
    IUserControlSession session,
    IDashboardService dashboardService,
    IUserService userService) : BaseController(session)
{
    public async Task<IActionResult> Index()
    {
        var today = DateTimeOffset.Now.DateTime;
        var viewModel = new DashboardViewModel
        {
            SummaryRequests = await dashboardService.GetMySpaceAsync(EmployeeSession.EmpId, today)
        };
        var isVerifierApprover = session.HasPermissionOnFunction(FunctionKey.VerifyRequest, Policy.Host)
                                 || session.HasPermissionOnFunction(FunctionKey.ApproveRequest, Policy.Host);
        var user = await userService.GetByIdAsync(session.UserId);
        ViewBag.IsResetPassword = user.IsResetPassword;
        ViewBag.IsVerifierApprover = isVerifierApprover;
        if (isVerifierApprover)
        {
            viewModel.MemberRequests = await dashboardService.GetMemberRequestAsync(EmployeeSession.EmpId, today);
        }

        return View(viewModel);
    }
}