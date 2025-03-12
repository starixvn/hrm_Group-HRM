using HRM.Services.Interfaces;

namespace HRM.Controllers.Export
{
    using System.Threading.Tasks;
    using Criteria.Report;
    using GO.UserControl.Client.Core.Session;
    using Controllers;
    using Resources;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using ViewModels.Report;

    public class ExportWeekendLeaveController(
        IUserControlSession userControlSession,
        IReportService reportService,
        IExportWeekendLeaveSummaryService exportWeekendLeaveSummaryService,
        IExportWeekendLeaveDetailService exportWeekendLeaveDetailService)
        : BaseController(userControlSession)
    {
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
            var data = exportWeekendLeaveSummaryService.Export(criteria, weekendLeaves);

            var fileName = $"{Labels.WeekendLeaveSummary}.xls";

            return File(data, "application/vnd.ms-excel", fileName);
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
            var data = exportWeekendLeaveDetailService.Export(criteria, weekendLeaveDetails);
            var fileName = $"{Labels.WeekendLeaveDetail}.xls";
            return File(data, "application/vnd.ms-excel", fileName);
        }
    }
}