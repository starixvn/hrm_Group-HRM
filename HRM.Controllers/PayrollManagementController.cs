using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Criteria.PayrollManagement;
using HRM.Services.Implement;
using HRM.Services.Implement.ActionLog;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class PayrollManagementController(
    IUserControlSession session,
    IAccessCounterService accessCounterService,
    IPayrollManagementService payrollService,
    IActionLogService actionLogService) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.PayrollManagement)]
    public async Task<IActionResult> Index()
    {
        await accessCounterService.UpdateCounterAsync(AccessCounterFunction.PayrollManagement);
        ViewBag.HasPermissionToUpload = session.HasPermissionOnFunction(FunctionKey.PayrollManagement, Policy.Host);
        return View();
    }

    [HttpGet]
    [HrmPermission(Policy.Read, FunctionKey.PayrollManagement)]
    public async Task<IActionResult> GetPayrollData(DateTime month, bool isLocal)
    {
        var criteria = new GetPayrollByMonthCriteria
        {
            CompanyId = CurrentCompanyId,
            Month = month,
            IsLocal = isLocal
        };

        var data = await payrollService.GetPayrollByMonthAsync(criteria);

        var objectId = $"{CurrentCompanyId}{month.Month}{month.Year}";
        var logDetail = await actionLogService.GetLogDetailByIdAsync(LogObjectType.Payroll, int.Parse(objectId), false);
        var latestLog = logDetail
            .OrderByDescending(l => l.LogDate)
            .Select(s => new KeyValuePair<string, DateTime>(s.Nickname, s.LogDate))
            .FirstOrDefault();

        return Json(new
        {
            data,
            latestLog
        });
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.PayrollManagement)]
    public async Task<IActionResult> GeneratePayroll(DateTime month)
    {
        var actorId = EmployeeSession.EmpId;
        var result = await payrollService.GeneratePayroll(month, actorId);
        return Json(result);
    }
}