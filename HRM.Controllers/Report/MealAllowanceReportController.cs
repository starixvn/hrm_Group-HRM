using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using HRM.Core.Constants;
    using HRM.Core.Filters;
    using HRM.Criteria.Report.MealAllowance;
    using HRM.Models.Report.MealAllowance;
    using HRM.Services.Implement.Report;
    using HRM.ViewModels.Report;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    [HrmPermission(Policy.Read, FunctionKey.MealAllowanceReport)]
    public class MealAllowanceReportController : BaseController
    {
        private readonly IMealAllowanceSummaryService reportService;

        public MealAllowanceReportController(
            IUserControlSession userControlSession,
            IMealAllowanceSummaryService reportService) : base(userControlSession)
        {
            this.reportService = reportService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var today = DateTimeOffset.Now.Date;
            var model = new MealAllowanceReportViewModel
            {
                FromDate = today,
                ToDate = today
            };

            return View("~/Views/Report/MealAllowance/Index.cshtml", model);
        }

        [HttpGet]
        public ActionResult MealAllowanceDetails(MealAllowanceReportViewModel model)
        {
            return View("~/Views/Report/MealAllowance/MealAllowanceDetails.cshtml", model);
        }

        [HttpGet]
        public ActionResult MealAllowanceDetailsGrid(MealAllowanceReportViewModel model)
        {
            var criteria = new GetMealAllowanceDetailReportCriteria
            {
                EmployeeId = model.EmployeeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                IsLocal = model.IsLocal,
                TeamId = model.TeamIds,
            };
            var mealAllowanceDetails = reportService.GetMealAllowanceDetails(criteria);
            return PartialView("~/Views/Report/MealAllowance/_MealAllowanceDetailsGrid.cshtml", mealAllowanceDetails);
        }

        public ActionResult GetMealAllowanceSummary(MealAllowanceReportViewModel model)
        {
            var criteria = new GetMealAllowanceSummaryReportCriteria
            {
                EmployeeId = model.EmployeeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                IsLocal = model.IsLocal,
                TeamId = model.TeamIds,
            };
            IEnumerable<MealAllowanceSummary> mealAllowanceSummary = reportService.GetMealAllowanceSummary(criteria);
            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;
            return PartialView("~/Views/Report/MealAllowance/_MealAllowanceSummary.cshtml", mealAllowanceSummary);
        }

        [HttpGet]
        public ActionResult ExportExcel(MealAllowanceReportViewModel model)
        {
            var criteriaSummary = new GetMealAllowanceSummaryReportCriteria
            {
                EmployeeId = model.EmployeeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                IsLocal = model.IsLocal,
                TeamId = model.TeamIds,
            };

            var criteriaDetails = new GetMealAllowanceDetailReportCriteria
            {
                EmployeeId = model.EmployeeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                IsLocal = model.IsLocal,
                TeamId = model.TeamIds,
            };
            IEnumerable<MealAllowanceSummary> mealAllowanceSummary = reportService.GetMealAllowanceSummary(criteriaSummary);
            IEnumerable<MealAllowanceDetail> mealAllowanceDetails = reportService.GetMealAllowanceDetails(criteriaDetails);
            byte[] bytes = reportService.Export(model, mealAllowanceSummary, mealAllowanceDetails);

            var fileName = $"Meal Allowance {model.FromDate.ToString("dd MMM yyyy")} - {model.ToDate:dd MMM yyyy}.xlsx";
            return File(bytes, MimeType.XLSX, fileName);
        }
    }
}