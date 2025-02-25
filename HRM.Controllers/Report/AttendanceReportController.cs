using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HRM.Core.Constants;
    using HRM.Criteria.Report;
    using HRM.Services;
    using HRM.ViewModels;
    using GO.UserControl.Client.Core.Session;

    public class AttendanceReportController : BaseController
    {
        private readonly IReportService reportService;
        private readonly IExportExcelAttendanceReportService excelService;
        private readonly ITeamService teamService;

        public AttendanceReportController(
            IUserControlSession userControlSession,
            IReportService reportSevice,
            IExportExcelAttendanceReportService excelService,
            ITeamService teamService) : base(userControlSession)
        {
            this.reportService = reportSevice;
            this.excelService = excelService;
            this.teamService = teamService;
        }

        public ActionResult Index()
        {
            var currentDateTime = DateTimeOffset.Now.DateTime;
            var model = new EmployeeAttendanceViewModel
            {
                FromDate = currentDateTime,
                ToDate = currentDateTime,
                TeamIds = string.Join(",", teamService.GetAll().Select(x => x.Id)),
                DateOption = 0,
                ExcludeDeletedUsers = true,
                SupervisorId = EmployeeSession.EmpId,
            };

            return View("~/Views/Report/Attendance/Index.cshtml", model);
        }

        [HttpPost]
        public ActionResult Details(EmployeeAttendanceViewModel model)
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
            IList<EmployeeAttendanceModel> employeeAttendances = reportService.GetEmployeeAttendanceReport(criteria);

            return PartialView("~/Views/Report/Attendance/_Details.cshtml", employeeAttendances);
        }

        public ActionResult ExportExcel(EmployeeAttendanceViewModel model)
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
            IList<EmployeeAttendanceModel> employeeAttendances = reportService.GetEmployeeAttendanceReport(criteria);
            byte[] bytes = excelService.Export(employeeAttendances, model.FromDate, model.ToDate);

            var fileName = $"AttendanceReport_{DateTimeOffset.Now:yyyyMMdd}.xlsx";
            return File(bytes, MimeType.XLSX, fileName);
        }
    }
}