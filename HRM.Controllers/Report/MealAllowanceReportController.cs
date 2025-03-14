namespace HRM.Controllers.Report;

using System;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Filters;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Criteria.Report.MealAllowance;
using HRM.Services.Implement.Report;
using HRM.ViewModels.Report;
using Microsoft.AspNetCore.Mvc;

[HrmPermission(Policy.Read, FunctionKey.MealAllowanceReport)]
public class MealAllowanceReportController(
    IUserControlSession userControlSession,
    IMealAllowanceSummaryService reportService) : BaseController(userControlSession)
{
    [HttpGet]
    public ActionResult Index()
    {
        var today = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
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
    public async Task<ActionResult> MealAllowanceDetailsGrid(MealAllowanceReportViewModel model)
    {
        var criteria = new GetMealAllowanceDetailReportCriteria
        {
            CompanyId = CurrentCompanyId,
            EmployeeId = model.EmployeeId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            IsLocal = model.IsLocal,
            TeamIds = model.TeamIds,
        };
        var mealAllowanceDetails = await reportService.GetMealAllowanceDetailsAsync(criteria);
        return PartialView("~/Views/Report/MealAllowance/_MealAllowanceDetailsGrid.cshtml", mealAllowanceDetails);
    }

    public async Task<ActionResult> GetMealAllowanceSummary(MealAllowanceReportViewModel model)
    {
        var criteria = new GetMealAllowanceSummaryReportCriteria
        {
            CompanyId = CurrentCompanyId,
            EmployeeId = model.EmployeeId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            IsLocal = model.IsLocal,
            TeamIds = model.TeamIds,
        };
        var mealAllowanceSummary = await reportService.GetMealAllowanceSummaryAsync(criteria);
        ViewBag.FromDate = model.FromDate;
        ViewBag.ToDate = model.ToDate;
        return PartialView("~/Views/Report/MealAllowance/_MealAllowanceSummary.cshtml", mealAllowanceSummary);
    }

    [HttpGet]
    public async Task<ActionResult> ExportExcel(MealAllowanceReportViewModel model)
    {
        var criteriaSummary = new GetMealAllowanceSummaryReportCriteria
        {
            CompanyId = CurrentCompanyId,
            EmployeeId = model.EmployeeId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            IsLocal = model.IsLocal,
            TeamIds = model.TeamIds
        };

        var criteriaDetails = new GetMealAllowanceDetailReportCriteria
        {
            CompanyId = CurrentCompanyId,
            EmployeeId = model.EmployeeId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            IsLocal = model.IsLocal,
            TeamIds = model.TeamIds
        };
        var mealAllowanceSummary = await reportService.GetMealAllowanceSummaryAsync(criteriaSummary);
        var mealAllowanceDetails = await reportService.GetMealAllowanceDetailsAsync(criteriaDetails);
        var bytes = reportService.Export(model, mealAllowanceSummary, mealAllowanceDetails);

        var fileName = $"Meal Allowance {model.FromDate:dd MMM yyyy} - {model.ToDate:dd MMM yyyy}.xlsx";
        return File(bytes, MimeType.XLSX, fileName);
    }
}