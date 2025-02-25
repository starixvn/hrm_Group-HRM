using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers.Export
{
    using System.Collections.Generic;
    using Criteria.Report;
    using GO.UserControl.Client.Core.Session;
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

        public ActionResult ExportWeekendLeaveSummaryReport(WeekendLeaveViewModel model)
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

            IList<WeekendLeaveSummary> weekendLeaves = reportService.GetWeekendLeaveSummary(criteria);
            exportWeekendLeaveSummaryService.Export(criteria, weekendLeaves);

            return new EmptyResult();
        }

        public ActionResult ExportWeekendLeaveDetailReport(WeekendLeaveViewModel model)
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

            IList<WeekendLeaveDetail> weekendLeaveDetails = reportService.GetWeekendLeaveDetail(criteria);
            exportWeekendLeaveDetailService.Export(criteria, weekendLeaveDetails);

            return new EmptyResult();
        }
    }
}