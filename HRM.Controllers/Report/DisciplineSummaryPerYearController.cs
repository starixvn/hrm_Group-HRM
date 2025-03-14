namespace HRM.Controllers.Report;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Filters;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Services;
using HRM.Criteria.Report;
using HRM.Models.Report.DisciplineSummaryPerYear;
using Services.ExportExcel;
using HRM.ViewModels.Report;
using Microsoft.AspNetCore.Mvc;
using Services.Implement;
using Services.Interfaces;


[HrmPermission(Policy.Read, FunctionKey.DisciplineSummaryPerYear)]
public class DisciplineSummaryPerYearController(
    IUserControlSession userControlSession,
    IBaseService baseService,
    IEmployeeService employeeService,
    IExportDisciplineSummaryPerYearService exportService,
    ICommonDataService commonDataService) : BaseController(userControlSession)
{
    public async Task<IActionResult> Index(DisciplineSummaryPerYearViewModel model)
    {
        const byte dateRange = 4;
        var today = DateTimeOffset.Now.DateTime.ToClientTimeZone();
        if (model != null)
        {
            model.ToYear = today.Year;
            model.FromYear = today.Year - dateRange;
        }
        ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

        return View("~/Views/Report/DisciplineSummaryPerYear/Index.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> GetDisciplineSummaryGrid(DisciplineSummaryPerYearViewModel model)
    {
        var employees = await GetDisciplineSummary(model);
        ViewBag.FromYear = model.FromYear;
        ViewBag.ToYear = model.ToYear;
        ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

        return PartialView("~/Views/Report/DisciplineSummaryPerYear/_Grid.cshtml", employees);
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel(DisciplineSummaryPerYearViewModel model)
    {
        var enableGroupDate = await commonDataService.IsEnableGroupDateAsync();
        var employees = await GetDisciplineSummary(model);
        var bytes = exportService.Export(model.FromYear, model.ToYear, employees, enableGroupDate);
        return File(bytes, MimeType.XLSX, "5_Years_Discipline_Summary.xlsx");
    }

    private async Task<IList<DisciplineSummaryPerYear>> GetDisciplineSummary(DisciplineSummaryPerYearViewModel model)
    {
        var criteria = new GetDisciplineSummaryPerYearCriteria
        {
            CompanyId = CurrentCompanyId,
            FromDate = new DateTimeOffset(model.FromYear, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
            ToDate = new DateTimeOffset(model.ToYear + 1, 1, 1, 0, 0, 0, TimeSpan.Zero).AddDays(-1).DateTime,
            TeamIds = model.TeamId,
            IsLocal = model.IsLocal,
            EmployeeId = model.EmployeeId,
            ApproverId = EmployeeSession.EmpId,
            IsShowAll = model.IsShowAll
        };
        var (employees, disciplines) = await baseService.FetchMultipleAsync<DisciplineSummaryPerYear, DisciplineSummaryPerYearDetail>(criteria);
		var disciplineSummaryPerYears = employees.ToList();
        foreach (var employee in disciplineSummaryPerYears)
        {
            var date = model.LengthOfService ? employee.CompanyDate : employee.JoinedDate;
            employee.LengthOfService = employeeService.CalculateLengthOfService(date, employee.MilitaryFromDate, employee.MilitaryToDate);
            employee.Disciplines = disciplines.Where(x => x.EmployeeId == employee.EmployeeId).ToList();
        }

        return disciplineSummaryPerYears.ToList();
    }
}