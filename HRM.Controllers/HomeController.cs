namespace HRM.Controllers; 

using System.Linq;
using Core.Enums;
using System;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using ViewModels.Dashboard;

public class HomeController(
    IUserControlSession userControlSession,
    IDashboardService dashboardService,
    IUserService userService) : BaseController(userControlSession)
{
    private readonly IUserControlSession userControlSession = userControlSession;

    public async Task<IActionResult> Index()
    {
        var today = DateTimeOffset.Now.DateTime.ToEndOfDay().ToServerTimeZone();
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
            var fromDate = DateTimeOffset.Now.DateTime.ToStartOfDay().ToClientTimeZone().Subtract(TimeSpan.FromDays(7));
            Parallel.ForEach(viewModel.MemberRequests, request =>
            {
                request.FromDate = fromDate;
                request.ToDate = today;

            });
        }

        return View(viewModel);
    }
}