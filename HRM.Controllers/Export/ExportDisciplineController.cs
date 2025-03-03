namespace HRM.Controllers
{
    using System.Threading.Tasks;
    using GO.UserControl.Client.Core.Session;
    using HRM.Criteria.Discipline;
    using HRM.Services;
    using HRM.ViewModels.Discipline;
    using Microsoft.AspNetCore.Mvc;
    using NPOI.HPSF;
    using HRM.Resources;
    using System;

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
            var data = exportDisciplineSummaryService.Export(criteria, disciplines);
            var fileName = $"{Labels.DisciplineSummaryReportName}.xls";
            return File(data, "application/vnd.ms-excel", fileName);
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
            var data = exportDisciplineDetailService.Export(criteria, disciplines);
            var fileName = $"{Labels.DisciplineDetailReportName}.xls";

            return File(data, "application/vnd.ms-excel", fileName);
        }
    }
}