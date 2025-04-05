namespace HRM.Controllers.Export;

using System.Threading.Tasks;
using Core.Constants;
using Criteria.Report;
using GO.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Mvc;
using Services.ExportExcel.WeekendLeave;
using Services.Interfaces;
using ViewModels.Report;

public class ExportWeekendLeaveController(
    IUserControlSession session,
    IReportService reportService,
    IExportWeekendLeaveSummaryService exportWeekendLeaveSummaryService,
    IExportWeekendLeaveDetailService exportWeekendLeaveDetailService) : BaseController(session)
{
    public async Task<ActionResult> ExportWeekendLeaveSummaryReport(WeekendLeaveViewModel model)
    {
        var criteria = new GetWeekendLeaveSummaryCriteria
        {
            CompanyId = CurrentCompanyId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamId = model.TeamId,
            IsLocal = model.IsLocal,
            EmployeeId = model.EmployeeId,
            MinTotalLeave = model.MinTotalLeave,
            SupervisorId = session.UserId
        };

        var weekendLeaves = await reportService.GetWeekendLeaveSummaryAsync(criteria);
        var data = exportWeekendLeaveSummaryService.Export(criteria, weekendLeaves);

        return File(data, MimeType.XLS, "WeekendLeaveSummary.xls");
    }

    public async Task<ActionResult> ExportWeekendLeaveDetailReport(WeekendLeaveViewModel model)
    {
        var criteria = new GetWeekendLeaveDetailCriteria
        {
            CompanyId = CurrentCompanyId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamId = model.TeamId,
            IsLocal = model.IsLocal,
            EmployeeId = model.EmployeeId,
            LeaveTypeId = model.LeaveTypeId
        };

        var weekendLeaveDetails = await reportService.GetWeekendLeaveDetailAsync(criteria);
        var data = exportWeekendLeaveDetailService.Export(criteria, weekendLeaveDetails);

        return File(data, MimeType.XLS, "WeekendLeaveDetail.xls");
    }
}