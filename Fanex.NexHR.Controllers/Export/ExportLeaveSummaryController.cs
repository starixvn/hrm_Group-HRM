namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using GO.UserControl.Client.Core.Session;
    using NPOI.HSSF.UserModel;

    public class ExportLeaveSummaryController : BaseExportController
    {
        private readonly IReportService reportService;
        private readonly IExportExcelService exportExcelService;
        private readonly IExportExcelUnpaidLeaveSummaryReportService exportExcelUnpaidLeaveSummaryReportService;

        public ExportLeaveSummaryController(
            IUserControlSession userControlSession,
            IReportService reportService,
            IExportExcelService exportExcelService,
            IExportExcelUnpaidLeaveSummaryReportService exportExcelUnpaidLeaveSummaryReportService) : base(userControlSession)
        {
            this.reportService = reportService;
            this.exportExcelService = exportExcelService;
            this.exportExcelUnpaidLeaveSummaryReportService = exportExcelUnpaidLeaveSummaryReportService;
        }

        public ActionResult AnnualLeaveSummary(LeaveSummaryViewModel model)
        {
            string fileName = $"LeaveSummary_{DateTimeOffset.Now:yyyyMMdd}.xls";
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
            IList<LeaveSummary> leaveSummary = reportService.GetAnnualLeaveBalanceReport(criteria);

            HSSFWorkbook hssfworkbook = exportExcelService.ExportLeaveSummary(leaveSummary, model.Year);

            return ExcelFile(hssfworkbook, fileName);
        }

#pragma warning disable S1541 // Methods and properties should not be too complex
#pragma warning disable S1067 // Expressions should not be too complex

        public ActionResult UnpaidLeaveSummary(LeaveSummaryViewModel model)
#pragma warning restore S1541 // Methods and properties should not be too complex
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
            IList<LeaveSummary> leaveSumaries = reportService.GetUnpaidLeave(criteria);
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

            exportExcelUnpaidLeaveSummaryReportService.Export(leaveSumaries, model.Year, model.LeaveType, model.Period);

            return new EmptyResult();
        }

#pragma warning restore S1067 // Expressions should not be too complex
    }
}