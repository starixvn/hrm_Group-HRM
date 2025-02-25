namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using GO.UserControl.Client.Core.Session;

    public class ExportCalendarController : BaseExportController
    {
        private readonly IWorkingCalendarService workingCalendarService;
        private readonly IWorkingCalendarReportService reportService;

        public ExportCalendarController(
            IUserControlSession userControlSession,
            IWorkingCalendarService workingCalendarService,
            IWorkingCalendarReportService reportService) : base(userControlSession)
        {
            this.workingCalendarService = workingCalendarService;
            this.reportService = reportService;
        }

        public ActionResult EmployeeWorkingCalendar(WorkingCalendarFilterViewModel model)
        {
            string fileName = "EmployeeWorkingCalendar_" + DateTimeOffset.Now.ToString("yyyyMMdd") + ".xlsx";
            var criteria = new GetWorkingCalendarWeeklyCriteria
            {
                EmpId = model.EmployeeId,
                Fromtime = model.FromDate,
                Totime = model.ToDate,
                TeamIds = model.TeamIds,
                JobTitleId = model.JobTitleIds,
                IsLocal = model.IsLocal
            };

            IEnumerable<WorkingCalendar> workingCalendars = workingCalendarService.GetWorkingCalendars(criteria);

            byte[] bytes = reportService.Export(criteria, workingCalendars);
            return File(bytes, MimeType.XLSX, fileName);
        }
    }
}