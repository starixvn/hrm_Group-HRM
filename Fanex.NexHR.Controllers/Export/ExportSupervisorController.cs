namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using GO.UserControl.Client.Core.Session;

    public class ExportSupervisorController : BaseExportController
    {
        private readonly IReportService reportService;
        private readonly IExportSupervisorService exportSupervisorService;

        public ExportSupervisorController(
            IUserControlSession userControlSession,
            IReportService reportService,
            IExportSupervisorService exportSupervisorService) : base(userControlSession)
        {
            this.reportService = reportService;
            this.exportSupervisorService = exportSupervisorService;
        }

        public ActionResult EmployeeSupervisor(EmployeeAndTeamSupervisorViewModel model)
        {
            var criteria = new GetSupervisorReportCriteria
            {
                EmployeeId = model.EmployeeId,
                TeamIds = model.TeamIds,
                IsLocal = model.IsLocal,
                ActionType = model.ActionType
            };
            IList<SupervisorReportModel> supervisors = reportService.GetSupervisorReport(criteria);
            byte[] bytes = exportSupervisorService.ExportEmployeeSupervisor(supervisors);

            return File(bytes, MimeType.XLSX, $"EmployeeSupervisor_{DateTimeOffset.Now:yyyyMMdd}.xlsx");
        }

        public ActionResult TeamSupervisor(EmployeeAndTeamSupervisorViewModel model)
        {
            var criteria = new GetTeamSupervisorCriteria
            {
                TeamIds = model.TeamIds,
            };
            IList<TeamSupervisorReportModel> supervisors = reportService.GetTeamSupervisor(criteria);
            byte[] bytes = exportSupervisorService.ExportTeamSupervisor(supervisors);

            return File(bytes, MimeType.XLSX, $"TeamSupervisor_{DateTimeOffset.Now:yyyyMMdd}.xlsx");
        }
    }
}