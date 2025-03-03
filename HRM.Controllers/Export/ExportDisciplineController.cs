namespace HRM.Controllers
{
    using System.Threading.Tasks;
    using GO.UserControl.Client.Core.Session;
    using HRM.Criteria.Discipline;
    using HRM.Services;
    using HRM.ViewModels.Discipline;
    using Microsoft.AspNetCore.Mvc;

    public class ExportDisciplineController(
        IUserControlSession userControlSession,
        IDisciplineService disciplineService,
        IExportDisciplineSummaryService exportDisciplineSummaryService,
        IExportDisciplineDetailService exportDisciplineDetailService) : BaseController(userControlSession)
    {
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