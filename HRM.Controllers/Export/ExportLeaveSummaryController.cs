namespace HRM.Controllers.Export;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Criteria.AnnualLeave;
using GO.UserControl.Client.Core.Session;
using HRM.Criteria.Report;
using HRM.Models;
using Microsoft.AspNetCore.Mvc;
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
        var fileName = $"LeaveSummary_{DateTimeOffset.Now:yyyyMMdd}.xls";
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
        var fileName = $"UnpaidLeaveSummary_{DateTimeOffset.Now:yyyyMMdd}.xls";
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

    private IEnumerable<LeaveSummary> FilterLeaveSummariesByPeriod(IEnumerable<LeaveSummary> leaveSummaries, string period)
    {
        return period switch
        {
            "1" => leaveSummaries.Where(x => x.Jan > 0 || x.Feb > 0 || x.Mar > 0 || x.Apr > 0 || x.May > 0 || x.Jun > 0).ToList(),
            "2" => leaveSummaries.Where(x => x.Jul > 0 || x.Aug > 0 || x.Sep > 0 || x.Oct > 0 || x.Nov > 0 || x.Dec > 0).ToList(),
            _ => leaveSummaries
        };
    }

}