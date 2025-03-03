using HRM.Services.Interfaces;

namespace HRM.Controllers.Export;

using System;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using Criteria;
using Criteria.AnnualLeave;
using Services;
using ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.ExportExcel;


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
            EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
            TeamIds = model.TeamIds,
            IsLocal = isLocal,
            Role = string.Empty,
            SupervisorId = null
        };
        var leaveSummary = await reportService.GetAnnualLeaveBalanceReportAsync(criteria);

        var workbook = exportExcelService.ExportLeaveSummary(leaveSummary, model.Year);

        return ExcelFile(workbook, fileName);
    }

    public async Task<IActionResult> UnpaidLeaveSummary(LeaveSummaryViewModel model)
    {
        var criteria = new GetUnpaidLeaveCriteria
        {
            Year = model.Year,
            EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
            TeamIds = model.TeamIds,
            IsLocal = (byte?)model.Category,
            SupervisorId = EmployeeSession.EmpId,
            LeaveType = model.LeaveType
        };
        var leaveSummaries = await reportService.GetUnpaidLeaveAsync(criteria);
        if (model.Period.Equals("1"))
        {
            leaveSummaries = leaveSummaries.Where(x => x.Jan > 0 || x.Feb > 0 || x.Mar > 0 || x.Apr > 0 || x.Mar > 0 || x.Jun > 0).ToList();
        }
        else
        {
            if (model.Period.Equals("2"))
            {
                leaveSummaries = leaveSummaries.Where(x => x.Jul > 0 || x.Aug > 0 || x.Sep > 0 || x.Oct > 0 || x.Nov > 0 || x.Dec > 0).ToList();
            }
        }

        var data = await exportReportService.Export(leaveSummaries, model.Year, model.LeaveType, model.Period);

        var fileName = $"UnpaidLeaveSummary_{DateTimeOffset.Now:yyyyMMdd}.xls";
        return File(data, "application/vnd.ms-excel", fileName);
    }
}