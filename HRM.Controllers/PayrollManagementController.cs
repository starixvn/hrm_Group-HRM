using System;
using System.Globalization;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Criteria.PayrollManagement;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class PayrollManagementController(
    IUserControlSession session,
    IPayrollManagementService payrollService) : BaseController(session)
{

    [HrmPermission(Policy.Read, FunctionKey.PayrollManagement)]
    public IActionResult Index()
    {
        ViewBag.HasPermissionToUpload = session.HasPermissionOnFunction(FunctionKey.PayrollManagement, Policy.Host);
        return View();
    }

    [HttpGet]
    [HrmPermission(Policy.Read, FunctionKey.PayrollManagement)]
    public async Task<IActionResult> GetPayrollData(string month)
    {
        var criteria = new GetPayrollByMonthCriteria
        {
            CompanyId = CurrentCompanyId,
            Month = !string.IsNullOrEmpty(month) ? DateTimeOffset.Parse(month, CultureInfo.InvariantCulture).DateTime : null,
        };
        var data = await payrollService.GetPayrollByMonthAsync(criteria);
        return Json(data);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.PayrollManagement)]
    public async Task<IActionResult> CalculatePayroll(DateTime date)
    {
        var result = await payrollService.GeneratePayroll(date);
        return Json(result);
    }
}