using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System;
    using HRM.Core.Services;
    using HRM.Criteria.EmployeeWorkFromHome;
    using HRM.Models;
    using HRM.Services;
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