namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using HRM.Criteria.Discipline;
    using HRM.Models.Discipline;
    using HRM.Services;
    using HRM.ViewModels.Discipline;
    using GO.UserControl.Client.Core.Session;
    using HRM.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class ExportDisciplineController : BaseController
    {
        private readonly IDisciplineService disciplineService;
        private readonly IExportDisciplineSummaryService exportDisciplineSummaryService;
        private readonly IExportDisciplineDetailService exportDisciplineDetailService;

        public ExportDisciplineController(
            IUserControlSession userControlSession,
            IDisciplineService disciplineService,
            IExportDisciplineSummaryService exportDisciplineSummaryService,
            IExportDisciplineDetailService exportDisciplineDetailService) : base(userControlSession)
        {
            this.disciplineService = disciplineService;
            this.exportDisciplineSummaryService = exportDisciplineSummaryService;
            this.exportDisciplineDetailService = exportDisciplineDetailService;
        }

        public async Task<IActionResult> ExportDisciplineSummaryReport(DisciplineSummaryViewModel model)
        {
            var criteria = new GetDisciplineSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TeamId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                ReasonId = model.ReasonId,
                ApproverId = EmployeeSession.EmpId,
                EmployeeName = model.Employee,
                TeamName = model.TeamName
            };

            var disciplines = await disciplineService.GetDisciplineSummaryAsync(criteria);
            exportDisciplineSummaryService.Export(criteria, disciplines);

            return new EmptyResult();
        }

        public async Task<ActionResult> ExportDisciplineDetailReport(DisciplineSummaryViewModel model)
        {
            var criteria = new GetDisciplineDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TeamId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                ReasonId = model.ReasonId,
                ApproverId = EmployeeSession.EmpId,
                EmployeeName = model.Employee,
                TeamName = model.TeamName
            };

            var disciplines = await disciplineService.GetDisciplineDetailAsync(criteria);
            exportDisciplineDetailService.Export(criteria, disciplines);

            return new EmptyResult();
        }
    }
}