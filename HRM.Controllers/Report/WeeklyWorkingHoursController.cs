namespace HRM.Controllers;

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
using HRM.Services;
using HRM.ViewModels.Report.WorkingHours;
using Microsoft.AspNetCore.Mvc;

[HrmPermission(Policy.Read, FunctionKey.AttendanceWeeklyReport)]
public class WeeklyWorkingHoursController(
    IUserControlSession userControlSession,
    IEmployeeWorkingHoursService workingHoursService) : BaseController(userControlSession)
{
    [HttpGet]
    public ActionResult Index()
    {
        var today = DateTime.Now.ToClientTimeZone().Date;
        var model = new WeeklyFilterViewModel
        {
            FromDate = today.FirstDayOfWeek(),
            ToDate = today.LastDayOfWeek(),
            EmployeeStatusId = (byte)EmployeeStatus.Enabled
        };

        return View("~/Views/Report/WeeklyWorkingHours/Index.cshtml", model);
    }

    [HttpGet]
    public async Task<IActionResult> GetWeeklyWorkingHours(WeeklyFilterViewModel model)
    {
        var param = new GetMonthlyWorkingHoursParam
        {
            ActorId = EmployeeSession.EmpId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamIds,
            Employee = model.Employee,
            IsLocal = model.IsLocal,
            EmployeeStatusId = model.EmployeeStatusId
        };
        var monthlyWorkings = await workingHoursService.GetMonthlyWorkingHoursAsync(param);
        ViewBag.FromDate = param.FromDate;
        ViewBag.ToDate = param.ToDate;

        return PartialView("~/Views/Report/WeeklyWorkingHours/_WeeklyWorkingHoursDetails.cshtml", monthlyWorkings);
    }

    [HttpGet]
    public async Task<ActionResult> ExportExcel(WeeklyFilterViewModel model)
    {
        var param = new GetMonthlyWorkingHoursParam
        {
            ActorId = EmployeeSession.EmpId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamIds,
            Employee = model.Employee,
            IsLocal = model.IsLocal,
            EmployeeStatusId = model.EmployeeStatusId
        };
        var monthlyWorkings = await workingHoursService.GetMonthlyWorkingHoursAsync(param);
        var bytes = workingHoursService.ExportWeeklyWorkingHours(param.FromDate, param.ToDate, monthlyWorkings);

        var fileName = $"{Labels.AttendanceWeeklyDetail} {param.FromDate:dd MMM yyyy} - {param.ToDate:dd MMM yyyy}.xlsx";
        return File(bytes, MimeType.XLSX, fileName);
    }
}