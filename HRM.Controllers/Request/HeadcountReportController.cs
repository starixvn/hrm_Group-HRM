using System;
using System.IO;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Services.Interfaces.Report.Headcount;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace HRM.Controllers.Request;

[HrmPermission(Policy.Read, FunctionKey.HeadcountReport)]
public class HeadcountReportController(
    IHeadcountReportService service,
    IHostEnvironment env,
    IUserControlSession session,
    IHeadcountExportService excelService)
    : BaseController(session)
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel(int year)
    {
        var data = await service.GetReportAsync(year);
        const string excelTemplatePath = "ExcelTemplates/HeadcountReportTemplate.xlsx";
        var templatePath = Path.Combine(env.ContentRootPath, excelTemplatePath);

        var bytes = excelService.Export(templatePath, data, year);
        var reportName = $"HeadcountReport_{year}_{DateTimeOffset.Now.Date:yyyyMMdd}";
        return File(bytes, "application/xlsx", $"{reportName}.xlsx");
    }
}