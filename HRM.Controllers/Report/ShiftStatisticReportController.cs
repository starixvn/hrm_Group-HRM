using System;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Models.Report.ShiftStatistic;
using HRM.Resources;
using HRM.Services.ExportExcel;
using HRM.Services.Implement.Report;
using HRM.Services.Interfaces;
using HRM.ViewModels.Report;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.Report;

[HrmPermission(Policy.Read, FunctionKey.ShiftStatisticReport)]
public class ShiftStatisticReportController(
    IUserControlSession session,
    IShiftStatisticReportService statisticReportService,
    IShiftService shiftService,
    IExportExcelShiftStatisticService exportExcelService,
    ITeamService teamService) : BaseController(session)
{
    public IActionResult Index()
    {
        var today = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        var model = new ShiftStatisticViewModel
        {
            FromDate = today.FirstDayOfWeek(),
            ToDate = today.LastDayOfWeek().Date
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
            IsLocal = model.IsLocal,
            CompanyId = CurrentCompanyId
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
            IsLocal = model.IsLocal,
            CompanyId = CurrentCompanyId
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
            IsLocal = model.IsLocal,
            CompanyId = CurrentCompanyId
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
            IsLocal = model.IsLocal,
            CompanyId = CurrentCompanyId
        };

        var shiftStatistics = await statisticReportService.GetShiftStatisticAsync(param);
        var exportData = exportExcelService.Export(shiftStatistics);

        var fileName = $"{Labels.ShiftStatisticReport}.xlsx";
        return File(exportData, MimeType.XLSX, fileName);
    }
}