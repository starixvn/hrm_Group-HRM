using System;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Filters;
using HRM.Models.Report.MonthlyWorkingHours;
using HRM.Resources;
using HRM.Services.Implement.Report;
using HRM.ViewModels.Report.WorkingHours;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.Report;

[HrmPermission(Policy.Read, FunctionKey.AttendanceMonthlyReport)]
public class MonthlyWorkingHoursController(
    IUserControlSession session,
    IEmployeeWorkingHoursService workingHoursService) : BaseController(session)
{
    [HttpGet]
    public ActionResult Index()
    {
        var today = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        var model = new MonthlyFilterViewModel
        {
            FromDate = today.FirstDayOfMonth(),
            ToDate = today.LastDayOfMonth(),
            EmployeeStatusIds = $"{(byte)EmployeeStatus.Enabled}"
        };

        return View("~/Views/Report/MonthlyWorkingHours/Index.cshtml", model);
    }

    [HttpGet]
    public async Task<IActionResult> GetMonthlyWorkingHours(MonthlyFilterViewModel model)
    {
        var param = new GetMonthlyWorkingHoursParam
        {
            CompanyId = CurrentCompanyId,
            ActorId = EmployeeSession.EmpId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamIds,
            Employee = model.Employee,
            IsLocal = model.IsLocal,
            EmployeeStatusIds = model.EmployeeStatusIds
        };
        var monthlyWorkings = await workingHoursService.GetMonthlyWorkingHoursAsync(param);
        ViewBag.FromDate = param.FromDate;
        ViewBag.ToDate = param.ToDate;

        return PartialView("~/Views/Report/MonthlyWorkingHours/_MonthlyWorkingHoursDetails.cshtml", monthlyWorkings);
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel(MonthlyFilterViewModel model)
    {
        var param = new GetMonthlyWorkingHoursParam
        {
            CompanyId = CurrentCompanyId,
            ActorId = EmployeeSession.EmpId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamIds,
            Employee = model.Employee,
            IsLocal = model.IsLocal,
            EmployeeStatusIds = model.EmployeeStatusIds
        };
        var bytes = await workingHoursService.ExportMonthlyWorkingHours(param);

        var fileName = $"{Labels.AttendanceMonthlyDetail} {param.FromDate:dd MMM yyyy} - {param.ToDate:dd MMM yyyy}.xlsx";
        return File(bytes, MimeType.XLSX, fileName);
    }
}