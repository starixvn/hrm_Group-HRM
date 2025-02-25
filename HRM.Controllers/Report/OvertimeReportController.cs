using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Filters;
    using HRM.Models.Report.Overtime;
    using HRM.Services;
    using HRM.ViewModels.Report.Overtime;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class OvertimeReportController : BaseController
    {
        private readonly IOvertimeReportService overtimeReportService;
        private readonly ITeamService teamService;

        public OvertimeReportController(
            IUserControlSession userControlSession,
            IOvertimeReportService overtimeReportService,
            ITeamService teamService) : base(userControlSession)
        {
            this.overtimeReportService = overtimeReportService;
            this.teamService = teamService;
        }

        [HrmPermission(Policy.Read, FunctionKey.MonthlyOvertimeReport)]
        public ActionResult MonthlyOvertime()
        {
            var model = new MonthlyFilterViewModel
            {
                EmployeeStatusId = (byte)EmployeeStatus.Enabled
            };

            return View("~/Views/Report/Overtime/MonthlyOvertime.cshtml", model);
        }

        [HttpGet]
        public ActionResult ListMonthlyOvertimeDetails(MonthlyFilterViewModel model)
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
            Tuple<IEnumerable<DateRange>, IEnumerable<DailyOvertimeDetails>> monthlyOvertime = overtimeReportService.GetMonthlyOvertime(param);

            return PartialView("~/Views/Report/Overtime/_ListMonthlyOvertimeDetails.cshtml", monthlyOvertime.Item2);
        }

        [HrmPermission(Policy.Read, FunctionKey.YearlyOvertimeReport)]
        public ActionResult YearlyOvertime(YearlyOvertimeViewModel model)
        {
            model.TeamIds = model.TeamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));
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
            model.Overtimes = overtimeReportService.GetYearlyOvertime(param);

            return View("~/Views/Report/Overtime/YearlyOvertime.cshtml", model);
        }

        [HrmPermission(Policy.Read, FunctionKey.MonthlyOvertimeReport)]
        public ActionResult MonthlyOvertimeDetails(GetMonthlyOvertimeDetailParam param)
        {
            IEnumerable<DailyOvertimeDetails> overtimes = overtimeReportService.GetMonthlyOvertimeDetails(param);
            ViewBag.Month = new DateTimeOffset(param.Year, param.Month, 1, 0, 0, 0, TimeSpan.Zero);

            return View("~/Views/Report/Overtime/MonthlyOvertimeDetails.cshtml", overtimes);
        }

        [HrmPermission(Policy.Read, FunctionKey.OvertimeReport)]
        public ActionResult DailyOvertime(DailyOvertimeDetailsViewModel model)
        {
            model.TeamIds = model.TeamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));
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
            model.Overtimes = overtimeReportService.GetDailyOvertimeDetails(param);

            return View("~/Views/Report/Overtime/DailyOvertime.cshtml", model);
        }

        [HrmPermission(Policy.Read, FunctionKey.MonthlyOvertimeReport)]
        public ActionResult ExportMonthlyOvertime(MonthlyFilterViewModel model)
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
            Tuple<IEnumerable<DateRange>, IEnumerable<DailyOvertimeDetails>> tuple = overtimeReportService.GetMonthlyOvertime(param);
            byte[] bytes = overtimeReportService.ExportMonthlyOvertime(tuple.Item1, tuple.Item2);

            var fileName = $"Monthly Overtime Report {model.FromDate:dd MMM yyyy} - {model.ToDate:dd MMM yyyy}.xlsx";

            return File(bytes, MimeType.XLSX, fileName);
        }

        [HrmPermission(Policy.Read, FunctionKey.YearlyOvertimeReport)]
        public ActionResult ExportYearlyOvertime(YearlyOvertimeViewModel model)
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
            IEnumerable<YearlyOvertime> overtimes = overtimeReportService.GetYearlyOvertime(param);
            byte[] bytes = overtimeReportService.ExportYearlyOvertime(overtimes, param);

            var fileName = $"Yearly overtime report - {param.Year}.xlsx";
            return File(bytes, MimeType.XLSX, fileName);
        }

        [HrmPermission(Policy.Read, FunctionKey.OvertimeReport)]
        public ActionResult ExportDailyOvertime(DailyOvertimeDetailsViewModel model)
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
            IEnumerable<DailyOvertimeDetails> overtimes = overtimeReportService.GetDailyOvertimeDetails(param);
            byte[] bytes = overtimeReportService.ExportDailyOvertimeDetails(overtimes, param);

            var fileName = string.Format(
                "Overtime Details Report from {0} to {1}.xlsx",
                param.FromDate.ToString("dd-MMM-yyyy"),
                param.ToDate.ToString("dd-MMM-yyyy"));
            return File(bytes, MimeType.XLSX, fileName);
        }
    }
}