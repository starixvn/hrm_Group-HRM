using System;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Filters;
using HRM.Models.Report.MonthlyWorkingHours;
using HRM.Resources;
using HRM.Services.Implement.Report;
using HRM.ViewModels.Report.WorkingHours;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.Report;

[HrmPermission(Policy.Read, FunctionKey.AttendanceDailyReport)]
public class DailyWorkingHoursController(
    IUserControlSession session,
    IEmployeeWorkingHoursService workingHoursService) : BaseController(session)
{
    [HttpGet]
    public ActionResult Index()
    {
        var model = new WeeklyFilterViewModel
        {
            FromDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date,
            EmployeeStatusIds = $"{(byte)EmployeeStatus.Enabled}"
        };

        return View("~/Views/Report/DailyWorkingHours/Index.cshtml", model);
    }

    [HttpGet]
    public async Task<IActionResult> GetDailyWorkingHours(WeeklyFilterViewModel model)
    {
        var param = new GetMonthlyWorkingHoursParam
        {
            CompanyId = CurrentCompanyId,
            ActorId = EmployeeSession.EmpId,
            FromDate = model.FromDate,
            ToDate = model.FromDate,
            TeamIds = model.TeamIds,
            Employee = model.Employee,
            IsLocal = model.IsLocal,
            EmployeeStatusIds = model.EmployeeStatusIds
        };
        ViewBag.FromDate = param.FromDate;
        var monthlyWorkings = await workingHoursService.GetMonthlyWorkingHoursAsync(param);

        return PartialView("~/Views/Report/DailyWorkingHours/_DailyWorkingHoursDetails.cshtml", monthlyWorkings);
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel(WeeklyFilterViewModel model)
    {
        var param = new GetMonthlyWorkingHoursParam
        {
            CompanyId = CurrentCompanyId,
            ActorId = EmployeeSession.EmpId,
            FromDate = model.FromDate,
            ToDate = model.FromDate,
            TeamIds = model.TeamIds,
            Employee = model.Employee,
            IsLocal = model.IsLocal,
            EmployeeStatusIds = model.EmployeeStatusIds
        };
        var monthlyWorkings = await workingHoursService.GetMonthlyWorkingHoursAsync(param);
        var bytes = workingHoursService.ExportDailyWorkingHours(param.FromDate, monthlyWorkings);

        var fileName = $"{Labels.AttendanceDailyDetail} {param.FromDate:dd MMM yyyy}.xlsx";
        return File(bytes, MimeType.XLSX, fileName);
    }
}