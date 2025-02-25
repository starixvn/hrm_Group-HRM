namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Report;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class ManpowerReportController : BaseController
    {
        private readonly IManpowerReportService reportService;

        public ManpowerReportController(
            IUserControlSession userControlSession,
            IManpowerReportService reportService) : base(userControlSession)
        {
            this.reportService = reportService;
        }

        [HRMPermission(Policy.Read, FunctionKey.ManpowerReport)]
        public ActionResult Manpower(int? year)
        {
            var model = new ManpowerViewModel
            {
                Year = year ?? DateTimeOffset.Now.Year
            };
            model.Manpowers = reportService.GetManpower(model.Year);

            return View("~/Views/Report/Manpower.cshtml", model);
        }

        [HRMPermission(Policy.Read, FunctionKey.ManpowerCategoryReport)]
        public ActionResult ManpowerCategory(int? year)
        {
            var model = new ManpowerCategoryViewModel
            {
                Year = year ?? DateTimeOffset.Now.Year
            };
            model.Manpowers = reportService.GetManpowerCategory(model.Year);

            return View("~/Views/Report/ManpowerCategory.cshtml", model);
        }

        [HRMPermission(Policy.Read, FunctionKey.ManpowerReport)]
        public ActionResult ExportManpower(int year)
        {
            IEnumerable<ManpowerModel> manpowers = reportService.GetManpower(year);
            byte[] bytes = reportService.ExportManpower(manpowers, year);

            var fileName = $"Manpower Report - {year}.xlsx";
            return File(bytes, MimeType.XLSX, fileName);
        }

        [HRMPermission(Policy.Read, FunctionKey.ManpowerCategoryReport)]
        public ActionResult ExportManpowerCategory(int year)
        {
            IEnumerable<ManpowerCategoryModel> manpowers = reportService.GetManpowerCategory(year);
            byte[] bytes = reportService.ExportManpowerCategory(manpowers, year);

            var fileName = $"Manpower Category Report - {year}.xlsx";
            return File(bytes, MimeType.XLSX, fileName);
        }
    }
}