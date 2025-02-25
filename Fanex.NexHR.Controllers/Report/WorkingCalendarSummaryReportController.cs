namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Report;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    [HRMPermission(Policy.Read, FunctionKey.WorkingCalendarReport)]
    public class WorkingCalendarSummaryReportController : BaseController
    {
        private readonly IWorkingCalendarSummaryReportService reportService;

        public WorkingCalendarSummaryReportController(
            IUserControlSession userControlSession,
            IWorkingCalendarSummaryReportService reportService) : base(userControlSession)
        {
            this.reportService = reportService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var today = DateTimeOffset.Now.Date;
            var model = new WorkingCalendarSummaryViewModel
            {
                FromDate = today,
                ToDate = today,
                EmployeeStatusId = (byte)EmployeeStatus.Enabled
            };

            return View("~/Views/Report/WorkingCalendarSummary.cshtml", model);
        }

        [HttpGet]
        public ActionResult GetWorkingCalendarSumary(WorkingCalendarSummaryViewModel model)
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
            IEnumerable<WorkingCalendarSummary> calendars = reportService.GetWorkingCalendarSummary(param);
            ViewBag.FromDate = param.FromDate;
            ViewBag.ToDate = param.ToDate;
            IEnumerable<WorkingCalendarSummary> employees = calendars.GroupBy(x => x.EmployeeId).Select(x => x.First()).OrderBy(x => x.Nickname);
            ViewBag.employee = employees;
            return PartialView("~/Views/Report/_WorkingCalendarSummaryDetail.cshtml", calendars);
        }

        [HttpGet]
        public ActionResult ExportExcel(WorkingCalendarSummaryViewModel model)
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
            IEnumerable<WorkingCalendarSummary> calendars = reportService.GetWorkingCalendarSummary(param);
            byte[] bytes = reportService.Export(param, calendars);

            var fileName = $"Working Calendar Summary {model.FromDate.ToString("dd MMM yyyy")} - {model.ToDate.ToString("dd MMM yyyy")}.xlsx";
            return File(bytes, MimeType.XLSX, fileName);
        }
    }
}