using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System;
    using System.Collections.Generic;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Filters;
    using HRM.Models.Report.MonthlyWorkingHours;
    using HRM.Resources;
    using HRM.Services;
    using HRM.ViewModels.Report.WorkingHours;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    [HrmPermission(Policy.Read, FunctionKey.AttendanceDailyReport)]
    public class DailyWorkingHoursController : BaseController
    {
        private readonly IEmployeeWorkingHoursService workingHoursService;

        public DailyWorkingHoursController(
            IUserControlSession userControlSession,
            IEmployeeWorkingHoursService workingHoursService) : base(userControlSession)
        {
            this.workingHoursService = workingHoursService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = new WeeklyFilterViewModel
            {
                FromDate = DateTimeOffset.Now.Date,
                EmployeeStatusId = (byte)EmployeeStatus.Enabled
            };

            return View("~/Views/Report/DailyWorkingHours/Index.cshtml", model);
        }

        [HttpGet]
        public ActionResult GetDailyWorkingHours(WeeklyFilterViewModel model)
        {
            var param = new GetMonthlyWorkingHoursParam
            {
                ActorId = EmployeeSession.EmpId,
                FromDate = model.FromDate,
                ToDate = model.FromDate,
                TeamIds = model.TeamIds,
                Employee = model.Employee,
                IsLocal = model.IsLocal,
                EmployeeStatusId = model.EmployeeStatusId
            };
            ViewBag.FromDate = param.FromDate;
            IEnumerable<MonthlyWorking> monthlyWorkings = workingHoursService.GetMonthlyWorkingHours(param);

            return PartialView("~/Views/Report/DailyWorkingHours/_DailyWorkingHoursDetails.cshtml", monthlyWorkings);
        }

        [HttpGet]
        public ActionResult ExportExcel(WeeklyFilterViewModel model)
        {
            var param = new GetMonthlyWorkingHoursParam
            {
                ActorId = EmployeeSession.EmpId,
                FromDate = model.FromDate,
                ToDate = model.FromDate,
                TeamIds = model.TeamIds,
                Employee = model.Employee,
                IsLocal = model.IsLocal,
                EmployeeStatusId = model.EmployeeStatusId
            };
            IEnumerable<MonthlyWorking> monthlyWorkings = workingHoursService.GetMonthlyWorkingHours(param);
            byte[] bytes = workingHoursService.ExportDailyWorkingHours(param.FromDate, monthlyWorkings);

            var fileName = $"{Labels.AttendanceDailyDetail} {param.FromDate:dd MMM yyyy}.xlsx";
            return File(bytes, MimeType.XLSX, fileName);
        }
    }
}