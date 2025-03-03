using HRM.Services.Interfaces;

namespace HRM.Controllers;

using System;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Models.Report;
using HRM.Resources;
using HRM.Services;
using HRM.ViewModels;
using Microsoft.AspNetCore.Mvc;

[HrmPermission(Policy.Read, FunctionKey.ShiftStatisticReport)]
public class ShiftStatisticReportController(
    IUserControlSession userControlSession,
    IShiftStatisticReportService statisticReportService,
    IShiftService shiftService,
    IExportExcelShiftStatisticService exportExcelService,
    ITeamService teamService) : BaseController(userControlSession)
{
    public IActionResult Index()
    {
        var model = new ShiftStatisticViewModel
        {
            FromDate = DateTimeOffset.Now.Date.FirstDayOfWeek(),
            ToDate = DateTimeOffset.Now.Date.LastDayOfWeek().Date
        };

        return View("~/Views/Report/ShiftStatistic/Index.cshtml", model);
    }

    public async Task<IActionResult> ShiftStatistic(ShiftStatisticViewModel model)
    {
        var param = new GetShiftStatisticParam
        {
            SupervisorId = EmployeeSession.EmpId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamIds,
            IsShowWholeCompany = model.IsShowWholeCompany,
            IsLocal = model.IsLocal
        };
        model.ShiftStatistics = await statisticReportService.GetShiftStatisticAsync(param);
        return PartialView("~/Views/Report/ShiftStatistic/_ShiftStatistic.cshtml", model);
    }

    public async Task<IActionResult> Details(ShiftStatisticDetailViewModel model)
    {
        model.Shifts = await shiftService.GetAllAsync();
        return View("~/Views/Report/ShiftStatistic/Details.cshtml", model);
    }

    public async Task<IActionResult> ShiftStatisticDetails(ShiftStatisticDetailViewModel model)
    {
        var teams = await teamService.GetAllAsync();
        var param = new GetShiftStatisticDetailParam
        {
            SupervisorId = EmployeeSession.EmpId,
            WorkDate = model.WorkDate,
            ShiftId = model.ShiftId,
            EmployeeId = model.EmployeeId,
            TeamIds = model.TeamIds ?? string.Join(",", teams.Select(x => x.Id)),
            IsShowWholeCompany = model.IsShowWholeCompany,
            IsLocal = model.IsLocal
        };
        var details = await statisticReportService.GetShiftStatisticDetailAsync(param);
        return PartialView("~/Views/Report/ShiftStatistic/_ShiftStatisticDetails.cshtml", details);
    }

    public async Task<IActionResult> ExportExcelStatisticDetails(ShiftStatisticDetailViewModel model)
    {
        var param = new GetShiftStatisticDetailParam
        {
            SupervisorId = EmployeeSession.EmpId,
            WorkDate = model.WorkDate,
            ShiftId = model.ShiftId,
            EmployeeId = model.EmployeeId,
            TeamIds = model.TeamIds,
            IsShowWholeCompany = model.IsShowWholeCompany,
            IsLocal = model.IsLocal
        };

        var details = await statisticReportService.GetShiftStatisticDetailAsync(param);
        var exportData = exportExcelService.ExportDetail(details);

        return File(exportData, MimeType.XLSX, "Shift statistic details report.xlsx");
    }

    public async Task<IActionResult> ExportExcel(ShiftStatisticViewModel model)
    {
        var param = new GetShiftStatisticParam
        {
            SupervisorId = EmployeeSession.EmpId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamIds,
            IsShowWholeCompany = model.IsShowWholeCompany,
            IsLocal = model.IsLocal
        };

        var shiftStatistics = await statisticReportService.GetShiftStatisticAsync(param);
        var exportData = exportExcelService.Export(shiftStatistics);

        var fileName = $"{Labels.ShiftStatisticReport}.xlsx";
        return File(exportData, MimeType.XLSX, fileName);
    }
}