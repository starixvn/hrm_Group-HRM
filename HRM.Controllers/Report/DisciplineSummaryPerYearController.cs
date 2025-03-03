namespace Fanex.NexHR.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HRM.Core.Constants;
    using HRM.Core.Filters;
    using HRM.Core.Services;
    using HRM.Criteria.Report;
    using HRM.Models.Report;
    using HRM.Services;
    using HRM.ViewModels.Report;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using HRM.Controllers;
    using HRM.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [HrmPermission(Policy.Read, FunctionKey.DisciplineSummaryPerYear)]
    public class DisciplineSummaryPerYearController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;
        private readonly IExportDisciplineSummaryPerYearService exportService;
        private readonly ICommonDataService commonDataService;

        public DisciplineSummaryPerYearController(
            IUserControlSession userControlSession,
            IBaseService baseService,
            IEmployeeService employeeService,
            IExportDisciplineSummaryPerYearService exportService,
            ICommonDataService commonDataService) : base(userControlSession)
        {
            this.baseService = baseService;
            this.employeeService = employeeService;
            this.exportService = exportService;
            this.commonDataService = commonDataService;
        }

        public async Task<IActionResult> Index(DisciplineSummaryPerYearViewModel model)
        {
            const byte DateRange = 4;
            if (model != null)
            {
                model.ToYear = DateTimeOffset.Now.Year;
                model.FromYear = DateTimeOffset.Now.Year - DateRange;
            }
            ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

            return View("~/Views/Report/DisciplineSummaryPerYear/Index.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> GetDisciplineSummaryGrid(DisciplineSummaryPerYearViewModel model)
        {
            IList<DisciplineSummaryPerYear> employees = await GetDisciplineSummary(model);
            ViewBag.FromYear = model.FromYear;
            ViewBag.ToYear = model.ToYear;
            ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

            return PartialView("~/Views/Report/DisciplineSummaryPerYear/_Grid.cshtml", employees);
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel(DisciplineSummaryPerYearViewModel model)
        {
            bool enableGroupDate = await commonDataService.IsEnableGroupDateAsync();
            var employees = await GetDisciplineSummary(model);
            byte[] bytes = exportService.Export(model.FromYear, model.ToYear, employees, enableGroupDate);
            return File(bytes, MimeType.XLSX, "5_Years_Discipline_Summary.xlsx");
        }

        private async Task<IList<DisciplineSummaryPerYear>> GetDisciplineSummary(DisciplineSummaryPerYearViewModel model)
        {
            var criteria = new GetDisciplineSummaryPerYearCriteria
            {
                FromDate = new DateTimeOffset(model.FromYear, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                ToDate = new DateTimeOffset(model.ToYear + 1, 1, 1, 0, 0, 0, TimeSpan.Zero).AddDays(-1).DateTime,
                TeamId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                ApproverId = EmployeeSession.EmpId,
                IsShowAll = model.IsShowAll
            };
            (var employees, var disciplines) = await baseService.FetchMultipleAsync<DisciplineSummaryPerYear, DisciplineSummaryPerYearDetail>(criteria);

            foreach (var employee in employees)
            {
                var date = model.LengthOfService ? employee.CompanyDate : employee.JoinedDate;
                employee.LengthOfService = employeeService.CaculateLengthOfService(date, employee.MilitaryFromDate, employee.MilitaryToDate);
                employee.Disciplines = disciplines.Where(x => x.EmployeeId == employee.EmployeeId).ToList();
            }

            return employees.ToList();
        }
    }
}