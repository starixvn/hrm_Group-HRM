using HRM.Core.Enums;
using HRM.Services.ExportExcel;
using HRM.ViewModels.Report;

namespace HRM.Controllers.Report;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using GO.UserControl.Client.Core.Session;
using HRM.Criteria.Report;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

[Route("report/attendance")]
public class AttendanceReportController(
    IUserControlSession session,
    IReportService reportService,
    IExportExcelAttendanceReportService excelService,
    IEmployeeService employeeService,
    ITeamService teamService) : BaseController(session)
{
    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var currentDateTime = DateTimeOffset.Now.DateTime.ToClientTimeZone();
        var teams = await teamService.GetAllAsync();
        var model = new EmployeeAttendanceViewModel
        {
            FromDate = currentDateTime,
            ToDate = currentDateTime,
            TeamIds = string.Join(",", teams.Select(x => x.Id)),
            DateOption = 0,
            SupervisorId = EmployeeSession.EmpId,
            EmployeeStatusIds = $"{(byte)EmployeeStatus.Enabled}"
        };

        return View("~/Views/Report/Attendance/Index.cshtml", model);
    }

    [HttpPost("Details")]
    public async Task<IActionResult> Details(EmployeeAttendanceViewModel model)
    {
        var filteredEmployees = await GetFilteredEmployeeAttendanceAsync(model);
        return PartialView("~/Views/Report/Attendance/_Details.cshtml", filteredEmployees);
    }

    [HttpGet("ExportExcel")]
    public async Task<IActionResult> ExportExcel(EmployeeAttendanceViewModel model)
    {
        var filteredEmployees = await GetFilteredEmployeeAttendanceAsync(model);
        var bytes = excelService.Export(filteredEmployees, model.FromDate, model.ToDate);

        var fileName = $"AttendanceReport_{DateTimeOffset.Now.DateTime.ToClientTimeZone():yyyyMMdd}.xlsx";
        return File(bytes, MimeType.XLSX, fileName);
    }

    private async Task<IEnumerable<EmployeeAttendanceModel>> GetFilteredEmployeeAttendanceAsync(EmployeeAttendanceViewModel model)
    {
        var criteria = new GetEmployeeAttendanceCriteria
        {
            SupervisorId = model.SupervisorId,
            Fromtime = model.FromDate,
            Totime = model.ToDate,
            AttendanceType = model.AttendanceType,
            EmpId = model.EmployeeId,
            TeamIds = model.TeamIds,
            IsLocal = model.IsLocal,
            IsRealDate = model.IsRealDate,
            CompanyId = CurrentCompanyId,
            EmployeeStatusIds = model.EmployeeStatusIds
        };
        
        var employeeAttendances = await reportService.GetEmployeeAttendanceReportAsync(criteria);
        return await employeeService.FilterByEmployeeDataPermission(employeeAttendances, e => e.EmpId, (short)SupervisorActionType.LeaderTakeover);
    }
}