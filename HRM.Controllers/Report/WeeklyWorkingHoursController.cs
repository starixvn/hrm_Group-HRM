using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System;
    using System.Collections.Generic;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Filters;
    using HRM.Models.Report.MonthlyWorkingHours;
    using HRM.Resources;
    using HRM.Services;
    using HRM.ViewModels.Report.WorkingHours;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    [HrmPermission(Policy.Read, FunctionKey.AttendanceWeeklyReport)]
    public class WeeklyWorkingHoursController : BaseController
    {
        private readonly IEmployeeWorkingHoursService workingHoursService;

        public WeeklyWorkingHoursController(
            IUserControlSession userControlSession,
            IEmployeeWorkingHoursService workingHoursService) : base(userControlSession)
        {
            this.workingHoursService = workingHoursService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var today = DateTimeOffset.Now.Date;
            var model = new WeeklyFilterViewModel
            {
                FromDate = today.FirstDayOfWeek(),
                ToDate = today.LastDayOfWeek(),
                EmployeeStatusId = (byte)EmployeeStatus.Enabled
            };

            return View("~/Views/Report/WeeklyWorkingHours/Index.cshtml", model);
        }

        [HttpGet]
        public ActionResult GetWeeklyWorkingHours(WeeklyFilterViewModel model)
        {
            var param = new GetMonthlyWorkingHoursParam
            {
                ActorId = EmployeeSession.EmpId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TeamIds = model.TeamIds,
                Employee = model.Employee,
                IsLocal = model.IsLocal,
                EmployeeStatusId = model.EmployeeStatusId
            };
            IEnumerable<MonthlyWorking> monthlyWorkings = workingHoursService.GetMonthlyWorkingHours(param);
            ViewBag.FromDate = param.FromDate;
            ViewBag.ToDate = param.ToDate;

            return PartialView("~/Views/Report/WeeklyWorkingHours/_WeeklyWorkingHoursDetails.cshtml", monthlyWorkings);
        }

        [HttpGet]
        public ActionResult ExportExcel(WeeklyFilterViewModel model)
        {
            var param = new GetMonthlyWorkingHoursParam
            {
                ActorId = EmployeeSession.EmpId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TeamIds = model.TeamIds,
                Employee = model.Employee,
                IsLocal = model.IsLocal,
                EmployeeStatusId = model.EmployeeStatusId
            };
            IEnumerable<MonthlyWorking> monthlyWorkings = workingHoursService.GetMonthlyWorkingHours(param);
            byte[] bytes = workingHoursService.ExportWeeklyWorkingHours(param.FromDate, param.ToDate, monthlyWorkings);

            var fileName = $"{Labels.AttendanceWeeklyDetail} {param.FromDate:dd MMM yyyy} - {param.ToDate:dd MMM yyyy}.xlsx";
            return File(bytes, MimeType.XLSX, fileName);
        }
    }
}