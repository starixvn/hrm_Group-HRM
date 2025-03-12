using System;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Services.Implement.Report;
using HRM.ViewModels.Report;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.Report;

public class ManpowerReportController(
    IUserControlSession userControlSession,
    IManpowerReportService reportService) : BaseController(userControlSession)
{
    [HrmPermission(Policy.Read, FunctionKey.ManpowerReport)]
    public async Task<IActionResult> Manpower(int? year)
    {
        var model = new ManpowerViewModel
        {
            Year = year ?? DateTimeOffset.Now.DateTime.ToClientTimeZone().Year
        };
        model.Manpowers = await reportService.GetManpowerAsync(model.Year);

        return View("~/Views/Report/Manpower.cshtml", model);
    }

    [HrmPermission(Policy.Read, FunctionKey.ManpowerCategoryReport)]
    public async Task<IActionResult> ManpowerCategory(int? year)
    {
        var model = new ManpowerCategoryViewModel
        {
            Year = year ?? DateTimeOffset.Now.DateTime.ToClientTimeZone().Year
        };
        model.Manpowers = await reportService.GetManpowerCategoryAsync(model.Year);

        return View("~/Views/Report/ManpowerCategory.cshtml", model);
    }

    [HrmPermission(Policy.Read, FunctionKey.ManpowerReport)]
    public async Task<IActionResult> ExportManpower(int year)
    {
        var manpowers = await reportService.GetManpowerAsync(year);
        var bytes = reportService.ExportManpower(manpowers, year);

        var fileName = $"Manpower Report - {year}.xlsx";
        return File(bytes, MimeType.XLSX, fileName);
    }

    [HrmPermission(Policy.Read, FunctionKey.ManpowerCategoryReport)]
    public async Task<IActionResult> ExportManpowerCategory(int year)
    {
        var manpowers = await reportService.GetManpowerCategoryAsync(year);
        byte[] bytes = reportService.ExportManpowerCategory(manpowers, year);

        var fileName = $"Manpower Category Report - {year}.xlsx";
        return File(bytes, MimeType.XLSX, fileName);
    }
}