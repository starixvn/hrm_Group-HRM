using System;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Filters;
using HRM.Models.Report.Overtime;
using HRM.Services;
using HRM.Services.Interfaces;
using HRM.ViewModels.Report.Overtime;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.Report;

public class OvertimeReportController(
    IUserControlSession userControlSession,
    IOvertimeReportService overtimeReportService,
    ITeamService teamService) : BaseController(userControlSession)
{
    [HrmPermission(Policy.Read, FunctionKey.MonthlyOvertimeReport)]
    public IActionResult MonthlyOvertime()
    {
        var model = new MonthlyFilterViewModel
        {
            EmployeeStatusId = (byte)EmployeeStatus.Enabled
        };

        return View("~/Views/Report/Overtime/MonthlyOvertime.cshtml", model);
    }

    [HttpGet]
    public async Task<IActionResult> ListMonthlyOvertimeDetails(MonthlyFilterViewModel model)
    {
        var param = new GetMonthlyOvertimeParam
        {
            ActorId = EmployeeSession.EmpId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            Employee = model.Employee,
            EmployeeStatusId = model.EmployeeStatusId,
            TeamIds = model.TeamIds,
            IsLocal = model.IsLocal
        };
        var monthlyOvertime = await overtimeReportService.GetMonthlyOvertimeAsync(param);

        return PartialView("~/Views/Report/Overtime/_ListMonthlyOvertimeDetails.cshtml", monthlyOvertime.Item2);
    }

    [HrmPermission(Policy.Read, FunctionKey.YearlyOvertimeReport)]
    public async Task<IActionResult> YearlyOvertime(YearlyOvertimeViewModel model)
    {
        var teams = await teamService.GetAllAsync();
        model.TeamIds = model.TeamIds ?? string.Join(",", teams.Select(x => x.Id));
        var param = new GetYearlyOvertimeParam
        {
            ActorId = EmployeeSession.EmpId,
            Year = model.Year,
            Calendar = model.Calendar,
            TeamIds = model.TeamIds,
            Employee = model.Employee,
            EmployeeStatusId = model.EmployeeStatusId,
            IsLocal = model.IsLocal
        };
        model.Overtimes = await overtimeReportService.GetYearlyOvertimeAsync(param);

        return View("~/Views/Report/Overtime/YearlyOvertime.cshtml", model);
    }

    [HrmPermission(Policy.Read, FunctionKey.MonthlyOvertimeReport)]
    public async Task<IActionResult> MonthlyOvertimeDetails(GetMonthlyOvertimeDetailParam param)
    {
        var overtimes = await overtimeReportService.GetMonthlyOvertimeDetailsAsync(param);
        ViewBag.Month = new DateTimeOffset(param.Year, param.Month, 1, 0, 0, 0, TimeSpan.Zero);

        return View("~/Views/Report/Overtime/MonthlyOvertimeDetails.cshtml", overtimes);
    }

    [HrmPermission(Policy.Read, FunctionKey.OvertimeReport)]
    public async Task<IActionResult> DailyOvertime(DailyOvertimeDetailsViewModel model)
    {
        var teams = await teamService.GetAllAsync();
        model.TeamIds = model.TeamIds ?? string.Join(",", teams.Select(x => x.Id));
        var param = new GetDailyOvertimeDetailsParam
        {
            ActorId = EmployeeSession.EmpId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamIds,
            Employee = model.Employee,
            IsLocal = model.IsLocal,
            EmployeeStatusId = model.EmployeeStatusId
        };
        model.Overtimes = await overtimeReportService.GetDailyOvertimeDetailsAsync(param);

        return View("~/Views/Report/Overtime/DailyOvertime.cshtml", model);
    }

    [HrmPermission(Policy.Read, FunctionKey.MonthlyOvertimeReport)]
    public async Task<IActionResult> ExportMonthlyOvertime(MonthlyFilterViewModel model)
    {
        var param = new GetMonthlyOvertimeParam
        {
            ActorId = EmployeeSession.EmpId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            Employee = model.Employee,
            EmployeeStatusId = model.EmployeeStatusId,
            TeamIds = model.TeamIds,
            IsLocal = model.IsLocal
        };
        var tuple = await overtimeReportService.GetMonthlyOvertimeAsync(param);
        var bytes = overtimeReportService.ExportMonthlyOvertime(tuple.Item1, tuple.Item2);

        var fileName = $"Monthly Overtime Report {model.FromDate:dd MMM yyyy} - {model.ToDate:dd MMM yyyy}.xlsx";

        return File(bytes, MimeType.XLSX, fileName);
    }

    [HrmPermission(Policy.Read, FunctionKey.YearlyOvertimeReport)]
    public async Task<IActionResult> ExportYearlyOvertime(YearlyOvertimeViewModel model)
    {
        var param = new GetYearlyOvertimeParam
        {
            ActorId = EmployeeSession.EmpId,
            Year = model.Year,
            Calendar = model.Calendar,
            TeamIds = model.TeamIds,
            Employee = model.Employee,
            EmployeeStatusId = model.EmployeeStatusId,
            IsLocal = model.IsLocal
        };
        var overtimes = await overtimeReportService.GetYearlyOvertimeAsync(param);
        var bytes = overtimeReportService.ExportYearlyOvertime(overtimes, param);

        var fileName = $"Yearly overtime report - {param.Year}.xlsx";
        return File(bytes, MimeType.XLSX, fileName);
    }

    [HrmPermission(Policy.Read, FunctionKey.OvertimeReport)]
    public async Task<IActionResult> ExportDailyOvertime(DailyOvertimeDetailsViewModel model)
    {
        var param = new GetDailyOvertimeDetailsParam
        {
            ActorId = EmployeeSession.EmpId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamIds,
            Employee = model.Employee,
            IsLocal = model.IsLocal,
            EmployeeStatusId = model.EmployeeStatusId
        };
        var overtimes = await overtimeReportService.GetDailyOvertimeDetailsAsync(param);
        var bytes = overtimeReportService.ExportDailyOvertimeDetails(overtimes, param);

        var fileName = $"Overtime Details Report from {param.FromDate:dd-MMM-yyyy} to {param.ToDate:dd-MMM-yyyy}.xlsx";
        return File(bytes, MimeType.XLSX, fileName);
    }
}