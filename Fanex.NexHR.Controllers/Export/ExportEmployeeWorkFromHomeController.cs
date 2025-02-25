namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using GO.UserControl.Client.Core.Session;
    using NPOI.HSSF.UserModel;

    public class ExportEmployeeWorkFromHomeController : BaseExportController
    {
        private readonly IBaseService baseService;
        private readonly IExportExcelService exportExcelService;

        public ExportEmployeeWorkFromHomeController(
            IUserControlSession userControlSession,
            IExportExcelService exportExcelService,
            IBaseService baseService) : base(userControlSession)
        {
            this.baseService = baseService;
            this.exportExcelService = exportExcelService;
        }

        public ActionResult EmployeeWorkFromHome(GetEmployeeWorkFromHomeCriteria criteria)
        {
            var employees = baseService.GetData<EmployeeWorkFromHome>(criteria);
            string fileName = "EmployeeWorkFromHome_" + DateTimeOffset.Now.ToString("yyyyMMdd") + ".xls";

            HSSFWorkbook hssfworkbook = exportExcelService.ExportEmployeeWfh(employees);

            return ExcelFile(hssfworkbook, fileName);
        }
    }
}