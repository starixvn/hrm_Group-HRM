namespace HRM.Controllers.Export
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Report;
    using GO.UserControl.Client.Core.Session;
    using HRM.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Models.Report;
    using Services;
    using ViewModels.Report;

    public class ExportWeekendLeaveController : BaseController
    {
        private readonly IReportService reportService;
        private readonly IExportWeekendLeaveSummaryService exportWeekendLeaveSummaryService;
        private readonly IExportWeekendLeaveDetailService exportWeekendLeaveDetailService;

        public ExportWeekendLeaveController(
            IUserControlSession userControlSession,
            IReportService reportService,
            IExportWeekendLeaveSummaryService exportWeekendLeaveSummaryService,
            IExportWeekendLeaveDetailService exportWeekendLeaveDetailService) : base(userControlSession)
        {
            this.reportService = reportService;
            this.exportWeekendLeaveSummaryService = exportWeekendLeaveSummaryService;
            this.exportWeekendLeaveDetailService = exportWeekendLeaveDetailService;
        }

        public async Task<ActionResult> ExportWeekendLeaveSummaryReport(WeekendLeaveViewModel model)
        {
            var criteria = new GetWeekendLeaveSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TeamId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                MinTotalLeave = model.MinTotalLeave,
                SupervisorId = EmployeeSession.EmpId
            };

            var weekendLeaves = await reportService.GetWeekendLeaveSummaryAsync(criteria);
            exportWeekendLeaveSummaryService.Export(criteria, weekendLeaves);

            return new EmptyResult();
        }

        public async Task<ActionResult> ExportWeekendLeaveDetailReport(WeekendLeaveViewModel model)
        {
            var criteria = new GetWeekendLeaveDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TeamId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                LeaveTypeId = model.LeaveTypeId
            };

            var weekendLeaveDetails = await reportService.GetWeekendLeaveDetailAsync(criteria);
            exportWeekendLeaveDetailService.Export(criteria, weekendLeaveDetails);

            return new EmptyResult();
        }
    }
}