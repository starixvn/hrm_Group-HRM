namespace HRM.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.UserControl.Client.Core.Session;
    using HRM.Criteria;
    using HRM.Criteria.AnnualLeave;
    using HRM.Services;
    using HRM.ViewModels;
    using Microsoft.AspNetCore.Mvc;

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
            var leaveSumaries = await reportService.GetUnpaidLeaveAsync(criteria);
            if (model.Period.Equals("1"))
            {
                leaveSumaries = leaveSumaries.Where(x => x.Jan > 0 || x.Feb > 0 || x.Mar > 0 || x.Apr > 0 || x.Mar > 0 || x.Jun > 0).ToList();
            }
            else
            {
                if (model.Period.Equals("2"))
                {
                    leaveSumaries = leaveSumaries.Where(x => x.Jul > 0 || x.Aug > 0 || x.Sep > 0 || x.Oct > 0 || x.Nov > 0 || x.Dec > 0).ToList();
                }
            }

            exportReportService.Export(leaveSumaries, model.Year, model.LeaveType, model.Period);

            return new EmptyResult();
        }

#pragma warning restore S1067 // Expressions should not be too complex
    }
}