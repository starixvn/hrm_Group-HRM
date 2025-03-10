namespace HRM.Controllers;

using System;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Filters;
using HRM.Models.Report;
using HRM.Services;
using HRM.ViewModels.Report;
using Microsoft.AspNetCore.Mvc;

[HrmPermission(Policy.Read, FunctionKey.WorkingCalendarReport)]
public class WorkingCalendarSummaryReportController(
    IUserControlSession userControlSession,
    IWorkingCalendarSummaryReportService reportService) : BaseController(userControlSession)
{
    [HttpGet]
    public ActionResult Index()
    {
        var today = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        var model = new WorkingCalendarSummaryViewModel
        {
            FromDate = today,
            ToDate = today,
            EmployeeStatusId = (byte)EmployeeStatus.Enabled
        };

        return View("~/Views/Report/WorkingCalendarSummary.cshtml", model);
    }

    [HttpGet]
    public async Task<ActionResult> GetWorkingCalendarSummary(WorkingCalendarSummaryViewModel model)
    {
        var param = new GetWorkingHoursSummaryParam
        {
            ActorId = EmployeeSession.EmpId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamIds,
            Employee = model.Employee,
            IsLocal = model.IsLocal,
            EmployeeStatusId = model.EmployeeStatusId
        };
        var calendars = await reportService.GetWorkingCalendarSummaryAsync(param);
        ViewBag.FromDate = param.FromDate;
        ViewBag.ToDate = param.ToDate;
        var employees = calendars.GroupBy(x => x.EmployeeId).Select(x => x.First()).OrderBy(x => x.Nickname);
        ViewBag.employee = employees;
        return PartialView("~/Views/Report/_WorkingCalendarSummaryDetail.cshtml", calendars);
    }

    [HttpGet]
    public async Task<ActionResult> ExportExcel(WorkingCalendarSummaryViewModel model)
    {
        var param = new GetWorkingHoursSummaryParam
        {
            ActorId = EmployeeSession.EmpId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamIds,
            Employee = model.Employee,
            IsLocal = model.IsLocal,
            EmployeeStatusId = model.EmployeeStatusId
        };
        var calendars = await reportService.GetWorkingCalendarSummaryAsync(param);
        var bytes = reportService.Export(param, calendars);

        var fileName = $"Working Calendar Summary {model.FromDate:dd MMM yyyy} - {model.ToDate:dd MMM yyyy}.xlsx";
        return File(bytes, MimeType.XLSX, fileName);
    }
}