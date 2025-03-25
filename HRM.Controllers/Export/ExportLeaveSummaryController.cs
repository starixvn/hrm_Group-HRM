namespace HRM.Controllers.Export;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Criteria.AnnualLeave;
using GO.UserControl.Client.Core.Session;
using HRM.Criteria.Report;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.ExportExcel;
using Services.Interfaces;
using ViewModels.AnnualLeave;

public class ExportLeaveSummaryController(
    IUserControlSession userControlSession,
    IReportService reportService,
    IExportExcelService exportExcelService,
    IExportExcelUnpaidLeaveSummaryReportService exportReportService) : BaseExportController(userControlSession)
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
        var leaveSummaries = await reportService.GetUnpaidLeaveAsync(criteria);
        leaveSummaries = FilterLeaveSummariesByPeriod(leaveSummaries, model.Period);

        var data = await exportReportService.Export(leaveSummaries, model.Year, model.LeaveType, model.Period, CurrentCompanyId);
        var fileName = $"UnpaidLeaveSummary_{DateTimeOffset.Now.DateTime.ToClientTimeZone():yyyyMMdd}.xls";
        return File(data, "application/vnd.ms-excel", fileName);
    }

    private GetUnpaidLeaveCriteria CreateUnpaidLeaveCriteria(LeaveSummaryViewModel model)
    {
        return new GetUnpaidLeaveCriteria
        {
            Year = model.Year,
            EmpId = model.EmployeeId > 0 ? new int?(model.EmployeeId) : null,
            TeamIds = model.TeamIds,
            IsLocal = (byte?)model.Category,
            SupervisorId = EmployeeSession.EmpId,
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
        return new[] { x.Jan, x.Feb, x.Mar, x.Apr, x.May, x.Jun }.Any(month => month > 0);
    }

    private static bool HasLeavesInSecondHalf(LeaveSummary x)
    {
        return new[] { x.Jul, x.Aug, x.Sep, x.Oct, x.Nov, x.Dec }.Any(month => month > 0);
    }
}