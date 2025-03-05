using System;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Services;
using HRM.Criteria.EmployeeWorkFromHome;
using HRM.Models;
using HRM.Services.ExportExcel;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.Export;

public class ExportEmployeeWorkFromHomeController(
    IUserControlSession userControlSession,
    IExportExcelService exportExcelService,
    IBaseService baseService)
    : BaseExportController(userControlSession)
{
    public async Task<IActionResult> EmployeeWorkFromHome(GetEmployeeWorkFromHomeCriteria criteria)
    {
        var employees = await baseService.GetDataAsync<EmployeeWorkFromHome>(criteria);
        var fileName = "EmployeeWorkFromHome_" + DateTimeOffset.Now.ToString("yyyyMMdd") + ".xls";

        var hssfWorkbook = exportExcelService.ExportEmployeeWfh(employees);

        return ExcelFile(hssfWorkbook, fileName);
    }
}