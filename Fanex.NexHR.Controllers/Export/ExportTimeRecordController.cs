namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Criteria.Time;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using GO.UserControl.Client.Core.Session;

    public class ExportTimeRecordController : BaseController
    {
        private readonly IReportService reportService;
        private readonly IExportExcelTimeRecordService exportTimeRecordService;

        public ExportTimeRecordController(
            IUserControlSession userControlSession,
            IReportService reportSevice,
            IExportExcelTimeRecordService exportTimeRecordService) : base(userControlSession)
        {
            this.reportService = reportSevice;
            this.exportTimeRecordService = exportTimeRecordService;
        }

        public ActionResult Export(TimeRecordViewModel model)
        {
            int? employeeId = null;
            var criteria = new GetTimeRecordCriteria
            {
                EmpId = model.EmployeeId == 0 ? employeeId : model.EmployeeId,
                SupervisorId = EmployeeSession.EmpId,
                TeamIds = model.TeamIds,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                IsLocal = model.IsLocal
            };

            IList<TimeRecordModel> timeRecords = reportService.GetTimeRecords(criteria);
            byte[] exportData = exportTimeRecordService.Export(timeRecords);

            var fileName = $"{Labels.TimeRecordReport}.xlsx";
            return File(exportData, MimeType.XLSX, fileName);
        }
    }
}