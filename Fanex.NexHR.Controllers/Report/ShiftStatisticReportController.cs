namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    [HRMPermission(Policy.Read, FunctionKey.ShiftStatisticReport)]
    public class ShiftStatisticReportController : BaseController
    {
        private readonly IShiftStatisticReportService statisticReportService;
        private readonly IShiftService shiftService;
        private readonly IExportExcelShiftStatisticService exportExcelService;
        private readonly ITeamService teamService;

        public ShiftStatisticReportController(
            IUserControlSession userControlSession,
            IShiftStatisticReportService statisticReportService,
            IShiftService shiftService,
            IExportExcelShiftStatisticService exportExcelService,
            ITeamService teamService) : base(userControlSession)
        {
            this.statisticReportService = statisticReportService;
            this.shiftService = shiftService;
            this.exportExcelService = exportExcelService;
            this.teamService = teamService;
        }

        public ActionResult Index()
        {
            var model = new ShiftStatisticViewModel
            {
                FromDate = DateTimeOffset.Now.Date.FirstDayOfWeek(),
                ToDate = DateTimeOffset.Now.Date.LastDayOfWeek().Date
            };

            return View("~/Views/Report/ShiftStatistic/Index.cshtml", model);
        }

        public ActionResult ShiftStatistic(ShiftStatisticViewModel model)
        {
            var param = new GetShiftStatisticParam
            {
                SupervisorId = EmployeeSession.EmpId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TeamIds = model.TeamIds,
                IsShowWholeCompany = model.IsShowWholeCompany,
                IsLocal = model.IsLocal
            };
            model.ShiftStatistics = statisticReportService.GetShiftStatistic(param);
            return PartialView("~/Views/Report/ShiftStatistic/_ShiftStatistic.cshtml", model);
        }

        public ActionResult Details(ShiftStatisticDetailViewModel model)
        {
            model.Shifts = shiftService.GetAll();
            return View("~/Views/Report/ShiftStatistic/Details.cshtml", model);
        }

        public ActionResult ShiftStatisticDetails(ShiftStatisticDetailViewModel model)
        {
            var param = new GetShiftStatisticDetailParam
            {
                SupervisorId = EmployeeSession.EmpId,
                WorkDate = model.WorkDate,
                ShiftId = model.ShiftId,
                EmployeeId = model.EmployeeId,
                TeamIds = model.TeamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id)),
                IsShowWholeCompany = model.IsShowWholeCompany,
                IsLocal = model.IsLocal
            };
            IEnumerable<ShiftStatisticDetail> details = statisticReportService.GetShiftStatisticDetail(param);
            return PartialView("~/Views/Report/ShiftStatistic/_ShiftStatisticDetails.cshtml", details);
        }

        public ActionResult ExportExcelStatisticDetails(ShiftStatisticDetailViewModel model)
        {
            var param = new GetShiftStatisticDetailParam
            {
                SupervisorId = EmployeeSession.EmpId,
                WorkDate = model.WorkDate,
                ShiftId = model.ShiftId,
                EmployeeId = model.EmployeeId,
                TeamIds = model.TeamIds,
                IsShowWholeCompany = model.IsShowWholeCompany,
                IsLocal = model.IsLocal
            };

            IEnumerable<ShiftStatisticDetail> details = statisticReportService.GetShiftStatisticDetail(param);
            byte[] exportData = exportExcelService.ExportDetail(details);

            var fileName = $"{Labels.ShiftStatisticReport}.xlsx";
            return File(exportData, MimeType.XLSX, fileName);
        }

        public ActionResult ExportExcel(ShiftStatisticViewModel model)
        {
            var param = new GetShiftStatisticParam
            {
                SupervisorId = EmployeeSession.EmpId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TeamIds = model.TeamIds,
                IsShowWholeCompany = model.IsShowWholeCompany,
                IsLocal = model.IsLocal
            };

            var shiftStatistics = statisticReportService.GetShiftStatistic(param);
            byte[] exportData = exportExcelService.Export(shiftStatistics);

            var fileName = $"{Labels.ShiftStatisticReport}.xlsx";
            return File(exportData, MimeType.XLSX, fileName);
        }
    }
}