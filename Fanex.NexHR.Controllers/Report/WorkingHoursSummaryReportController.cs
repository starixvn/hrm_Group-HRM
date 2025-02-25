namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.Report.Overtime;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Report;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    [HRMPermission(Policy.Read, FunctionKey.AttendanceStatisticReport)]
    public class WorkingHoursSummaryReportController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IWorkingHoursSummaryReportService reportService;
        private readonly IOvertimeReportService overtimeReportService;
        private readonly ITeamService teamService;

        public WorkingHoursSummaryReportController(
            IUserControlSession userControlSession,
            IWorkingHoursSummaryReportService reportService,
            IOvertimeReportService overtimeReportService,
            ITeamService teamService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.reportService = reportService;
            this.overtimeReportService = overtimeReportService;
            this.teamService = teamService;
        }

        [HttpGet]
        public ActionResult Index(WorkingHoursSummaryViewModel model)
        {
            model.TeamIds = model.TeamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));
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
            model.WorkingHours = reportService.GetWorkingHoursSummary(param);

            ViewBag.IsHR = userControlSession.HasPermissionOnFunction(FunctionKey.ProcessRequest, Policy.Host);

            return View("~/Views/Report/WorkingHoursSummary.cshtml", model);
        }

        [HttpGet]
        public ActionResult ExportExcel(WorkingHoursSummaryViewModel model)
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
            IEnumerable<WorkingHoursSummary> workingHours = reportService.GetWorkingHoursSummary(param);
            IEnumerable<DailyOvertimeDetails> overtimes = GetOvertimeDetails(model);
            byte[] bytes = reportService.Export(param, workingHours, overtimes);

            return File(bytes, MimeType.XLSX, "Attendance Statistic for Payroll.xlsx");
        }

        private IEnumerable<DailyOvertimeDetails> GetOvertimeDetails(WorkingHoursSummaryViewModel model)
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
            return overtimeReportService.GetDailyOvertimeDetails(param);
        }
    }
}