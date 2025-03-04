namespace HRM.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using HRM.Core.Constants;
    using HRM.Core.Filters;
    using HRM.Models.Report;
    using HRM.Models.Report.Overtime;
    using HRM.Services;
    using HRM.ViewModels.Report;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using HRM.Controllers;
    using HRM.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [HrmPermission(Policy.Read, FunctionKey.AttendanceStatisticReport)]
    public class WorkingHoursSummaryReportController(
        IUserControlSession userControlSession,
        IWorkingHoursSummaryReportService reportService,
        IOvertimeReportService overtimeReportService,
        ITeamService teamService) : BaseController(userControlSession)
    {
        [HttpGet]
        public async Task<IActionResult> Index(WorkingHoursSummaryViewModel model)
        {
            var teams = await teamService.GetAllAsync();
            model.TeamIds = model.TeamIds ?? string.Join(",", teams.Select(x => x.Id));
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
            model.WorkingHours = await reportService.GetWorkingHoursSummaryAsync(param);

            ViewBag.IsHR = userControlSession.HasPermissionOnFunction(FunctionKey.ProcessRequest, Policy.Host);

            return View("~/Views/Report/WorkingHoursSummary.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel(WorkingHoursSummaryViewModel model)
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
            var workingHours = await reportService.GetWorkingHoursSummaryAsync(param);
            var overtimes = await GetOvertimeDetails(model);
            byte[] bytes = reportService.Export(param, workingHours, overtimes);

            return File(bytes, MimeType.XLSX, "Attendance Statistic for Payroll.xlsx");
        }

        private async Task<IEnumerable<DailyOvertimeDetails>> GetOvertimeDetails(WorkingHoursSummaryViewModel model)
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
            return await overtimeReportService.GetDailyOvertimeDetailsAsync(param);
        }
    }
}