using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Services;
using HRM.Criteria.EmployeePayroll;
using HRM.Criteria.PayrollManagement;
using HRM.Models.Payroll;
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
}