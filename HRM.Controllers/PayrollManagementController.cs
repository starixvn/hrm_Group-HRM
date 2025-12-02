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
using HRM.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class PayrollManagementController(
    IUserControlSession session,
    IAccessCounterService accessCounterService,
    IPayrollManagementService payrollService,
    IActionLogService actionLogService,
    IUnitService unitService,
    IDepartmentService departmentService,
    ICommonDataService commonDataService) : BaseController(session)
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
    public async Task<IActionResult> GetPayrollData(DateTime month, bool isLocal, string unitIds, string departmentIds, string teamIds)
    {
        var criteria = new GetPayrollByMonthCriteria
        {
            CompanyId = CurrentCompanyId,
            Month = month,
            IsLocal = isLocal,
            UnitIds = unitIds,
            DepartmentIds = departmentIds,
            TeamIds = teamIds
        };

        var data = await payrollService.GetPayrollByMonthAsync(criteria);

        var objectId = $"{CurrentCompanyId}{month.Month}{month.Year}";
        var logDetail = await actionLogService.GetLogDetailByIdAsync(LogObjectType.Payroll, int.Parse(objectId), false);
        KeyValuePair<string, DateTime>? latestLog = null;
        if (logDetail.Any())
        {
            latestLog = logDetail
                .OrderByDescending(l => l.LogDate)
                .Select(s => new KeyValuePair<string, DateTime>(s.Nickname, s.LogDate))
                .FirstOrDefault();
        }
        
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

    [HttpGet]
    public async Task<IActionResult> GetOrgChart()
    {
        var units = await unitService.GetUnitAsync();
        var departments = await departmentService.GetDepartmentAsync();
        var teams = await commonDataService.GetAllTeamAsync(companyId: CurrentCompanyId);

        departments = departments
            .Where(d => teams
                .Any(t => t.Department != null && t.Department.Id == d.Id))
            .ToList();
        units = units
            .Where(u => departments
                .Any(d => d.Unit != null && d.Unit.Id == u.Id))
            .ToList();

        var unitChart = units.Select(u => new MultiSelectViewModel
        {
            Id = u.Id,
            Text = u.Code,
            ParentId = u.Id,
            IsLeaf = true
        });

        var departmentChart = departments.Select(d => new MultiSelectViewModel
        {
            Id = d.Id,
            Text = d.Code,
            ParentId = d.Id,
            IsLeaf = true
        });

        var teamChart = units.Select(u => new MultiSelectViewModel
        {
            Id = u.Id,
            Text = u.Code,
            Items = departments.Where(d => d.Unit != null && d.Unit.Id == u.Id).Select(d => new MultiSelectViewModel
            {
                Id = d.Id,
                Text = d.Name,
                ParentId = u.Id,
                Items = teams.Where(s => s.Department != null && s.Department.Id == d.Id).Select(s => new MultiSelectViewModel
                {
                    Id = s.Id,
                    Text = s.Name,
                    IsLeaf = true,
                    ParentId = d.Id,
                }).ToList()
            }).ToList()
        });

        return Json(new { 
            team = teamChart,
            department = departmentChart,
            unit = unitChart
        });
    }
}