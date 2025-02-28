namespace HRM.Controllers.Report;

using System;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Criteria.Report;
using HRM.Services;
using HRM.ViewModels;
using Microsoft.AspNetCore.Mvc;

[Route("report/attendance")]
public class AttendanceReportController(
    IUserControlSession userControlSession,
    IReportService reportSevice,
    IExportExcelAttendanceReportService excelService,
    ITeamService teamService) : BaseController(userControlSession)
{
    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var currentDateTime = DateTimeOffset.Now.DateTime;
        var teams = await teamService.GetAllAsync();
        var model = new EmployeeAttendanceViewModel
        {
            FromDate = currentDateTime,
            ToDate = currentDateTime,
            TeamIds = string.Join(",", teams.Select(x => x.Id)),
            DateOption = 0,
            ExcludeDeletedUsers = true,
            SupervisorId = EmployeeSession.EmpId,
        };

        return View("~/Views/Report/Attendance/Index.cshtml", model);
    }

    [HttpPost("Details")]
    public async Task<IActionResult> Details(EmployeeAttendanceViewModel model)
    {
        var criteria = new GetEmployeeAttendanceCriteria
        {
            SupervisorId = model.SupervisorId,
            Fromtime = model.FromDate,
            Totime = model.ToDate,
            AttendanceType = model.AttendanceType,
            EmpId = model.EmployeeId,
            TeamIds = model.TeamIds,
            ExcludeUserDelete = model.ExcludeDeletedUsers ? 1 : 0,
            IsLocal = model.IsLocal,
            IsRealDate = model.IsRealDate
        };
        var employeeAttendances = await reportSevice.GetEmployeeAttendanceReportAsync(criteria);

        return PartialView("~/Views/Report/Attendance/_Details.cshtml", employeeAttendances);
    }

    [HttpGet("ExportExcel")]
    public async Task<IActionResult> ExportExcel(EmployeeAttendanceViewModel model)
    {
        var criteria = new GetEmployeeAttendanceCriteria
        {
            SupervisorId = model.SupervisorId,
            Fromtime = model.FromDate,
            Totime = model.ToDate,
            AttendanceType = model.AttendanceType,
            EmpId = model.EmployeeId,
            TeamIds = model.TeamIds,
            ExcludeUserDelete = model.ExcludeDeletedUsers ? 1 : 0,
            IsLocal = model.IsLocal,
            IsRealDate = model.IsRealDate
        };
        var employeeAttendances = await reportSevice.GetEmployeeAttendanceReportAsync(criteria);
        var bytes = excelService.Export(employeeAttendances, model.FromDate, model.ToDate);

        var fileName = $"AttendanceReport_{DateTimeOffset.Now:yyyyMMdd}.xlsx";
        return File(bytes, MimeType.XLSX, fileName);
    }
}