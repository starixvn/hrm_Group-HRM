using System;
using System.IO;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Services.ExportExcel;
using HRM.Services.Interfaces.Report.Headcount;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace HRM.Controllers.Request;

[HrmPermission(Policy.Read, FunctionKey.HeadcountReport)]
public class HeadcountReportController(
    IHeadcountReportService service,
    IHostEnvironment env,
    IHeadcountExportService excelService)
    : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel(int year)
    {
        var data = await service.GetReportAsync(year);
        const string excelTemplatePath = "ExcelTemplates/HeadCountReportTemplate.xlsx";
        var templatePath = Path.Combine(env.ContentRootPath, excelTemplatePath);

        var bytes = excelService.Export(templatePath, data);
        var reportName = $"HeadcountReport_{year}_{DateTime.Today:yyyyMMdd}";
        return File(bytes, "application/xlsx", $"{reportName}.xlsx");
    }
}