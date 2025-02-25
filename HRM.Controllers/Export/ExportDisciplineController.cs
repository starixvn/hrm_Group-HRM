using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System.Collections.Generic;
    using HRM.Criteria.Discipline;
    using HRM.Models.Discipline;
    using HRM.Services;
    using HRM.ViewModels.Discipline;
    using GO.UserControl.Client.Core.Session;

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

        public ActionResult ExportDisciplineSummaryReport(DisciplineSummaryViewModel model)
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

            IList<DisciplineSummaryModel> disciplines = disciplineService.GetDisciplineSummary(criteria);
            exportDisciplineSummaryService.Export(criteria, disciplines);

            return new EmptyResult();
        }

        public ActionResult ExportDisciplineDetailReport(DisciplineSummaryViewModel model)
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

            IList<DisciplineDetailModel> disciplines = disciplineService.GetDisciplineDetail(criteria);
            exportDisciplineDetailService.Export(criteria, disciplines);

            return new EmptyResult();
        }
    }
}