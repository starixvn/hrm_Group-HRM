namespace HRM.Controllers.Export;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Criteria.AnnualLeave;
using GO.UserControl.Client.Core.Session;
using HRM.Criteria.Report;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.ExportExcel;
using Services.Interfaces;
using ViewModels.AnnualLeave;

public class ExportLeaveSummaryController(
    IUserControlSession session,
    IReportService reportService,
    IExportExcelService exportExcelService,
    IExportExcelUnpaidLeaveSummaryReportService exportReportService) : BaseExportController(session)
{
    public async Task<IActionResult> AnnualLeaveSummary(LeaveSummaryViewModel model)
    {
        var fileName = $"LeaveSummary_{DateTimeOffset.Now.DateTime.ToClientTimeZone():yyyyMMdd}.xls";
        byte? isLocal = null;
        if (model.Category != null)
        {
            isLocal = (byte)model.Category;
        }

        var criteria = new GetAnnualLeaveBalanceCriteria
        {
            Year = model.Year,
            EmpId = model.EmployeeId > 0 ? new int?(model.EmployeeId) : null,
            TeamIds = model.TeamIds,
            IsLocal = isLocal,
            Role = string.Empty,
            SupervisorId = null,
            CompanyId = CurrentCompanyId
        };
        var leaveSummary = await reportService.GetAnnualLeaveBalanceReportAsync(criteria);

        var workbook = exportExcelService.ExportLeaveSummary(leaveSummary, model.Year);

        return ExcelFile(workbook, fileName);
    }

    public async Task<IActionResult> UnpaidLeaveSummary(LeaveSummaryViewModel model)
    {
        var criteria = CreateUnpaidLeaveCriteria(model);
        var leaveSummaries = await reportService.GetUnpaidLeaveAsync(criteria, model.Period);

        var data = await exportReportService.Export(leaveSummaries, model.Year, model.LeaveType, model.Period, CurrentCompanyId);
        var fileName = $"UnpaidLeaveSummary_{DateTimeOffset.Now.DateTime.ToClientTimeZone():yyyyMMdd}.xls";
        return File(data, MimeType.XLS, fileName);
    }

    private GetUnpaidLeaveCriteria CreateUnpaidLeaveCriteria(LeaveSummaryViewModel model)
    {
        return new GetUnpaidLeaveCriteria
        {
            Year = model.Year,
            EmpId = model.EmployeeId > 0 ? new int?(model.EmployeeId) : null,
            TeamIds = model.TeamIds,
            IsLocal = (byte?)model.Category,
            SupervisorId = session.UserId,
            LeaveType = model.LeaveType,
            CompanyId = CurrentCompanyId
        };
    }

    private static IEnumerable<LeaveSummary> FilterLeaveSummariesByPeriod(IEnumerable<LeaveSummary> leaveSummaries, string period)
    {
        return period switch
        {
            "1" => leaveSummaries.Where(HasLeavesInFirstHalf).ToList(),
            "2" => leaveSummaries.Where(HasLeavesInSecondHalf).ToList(),
            _ => leaveSummaries
        };
    }

    private static bool HasLeavesInFirstHalf(LeaveSummary x)
    {
        return Array.Exists([x.Jan, x.Feb, x.Mar, x.Apr, x.May, x.Jun], month => month > 0);
    }

    private static bool HasLeavesInSecondHalf(LeaveSummary x)
    {
        return Array.Exists([x.Jul, x.Aug, x.Sep, x.Oct, x.Nov, x.Dec], month => month > 0);
    }
}