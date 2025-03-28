namespace HRM.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Filters;
using Criteria.HospitalizationLeave;
using Export;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Models.HospitalizationLeave;
using Resources;
using Services.ExportExcel;
using Services.Interfaces;
using ViewModels.AnnualLeave;
using ViewModels.HospitalizationLeave;

[HrmPermission(Policy.Read, FunctionKey.HospitalizationLeaveBalance)]
public class HospitalizationLeaveBalanceController(
    IUserControlSession session,
    IEmployeeService employeeService,
    ITeamService teamService,
    IBaseService baseService,
    IExportExcelService exportExcelService) : BaseExportController(session)
{
    public async Task<IActionResult> Index(
        string teamIds,
        string employee,
        int? year = null,
        int? employeeId = null,
        bool? isLocal = null)
    {
        var model = new HospitalizationLeaveBalanceViewModel
        {
            Year = year ?? DateTimeOffset.Now.DateTime.ToClientTimeZone().Year,
            TeamIds = teamIds,
            Employee = employee,
            EmployeeId = employeeId,
            IsLocal = isLocal,
            IsHost = session.HasPermissionOnFunction(FunctionKey.HospitalizationLeaveBalance, Policy.Host)
        };

        return await Index(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(HospitalizationLeaveBalanceViewModel model)
    {
        model.Teams = await teamService.GetBySupervisorAsync(session.UserId);
        model.TeamIds = !string.IsNullOrWhiteSpace(model.TeamIds) ? model.TeamIds : string.Join(",", model.Teams.Select(x => x.Id));
        var criteria = new GetSummaryReportCriteria
        {
            Year = model.Year,
            EmployeeId = model.EmployeeId,
            TeamIds = model.TeamIds,
            IsLocal = model.IsLocal,
            SupervisorId = session.UserId,
            CompanyId = CurrentCompanyId
        };
        model.Employees = await baseService.GetDataAsync<HospitalizationLeaveBalanceSummaryReport>(criteria);

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.HospitalizationLeaveBalance)]
    public async Task<IActionResult> Adjust([FromBody] AnnualLeaveBalanceMultiViewModel model)
    {
        var criteria = new InsertMultipleTransactionCriteria
        {
            EmployeeIds = model.EmpIds,
            Description = model.Description,
            CreditDays = model.CreditDays,
            DeductDays = model.DeductDays,
            FromDate = model.AffectedFrom,
            ActorId = session.UserId
        };
        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return Json(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.HospitalizationLeaveBalance)]
    public async Task<IActionResult> Reset([FromBody] ResetMultipleTransactionCriteria criteria)
    {
        criteria.ActorId = session.UserId;
        criteria.Year = DateTimeOffset.Now.DateTime.ToClientTimeZone().AddYears(-1).Year;
        criteria.CompanyId = CurrentCompanyId;
        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return Json(commandResult);
    }

    [HrmPermission(Policy.Host, FunctionKey.HospitalizationLeaveBalance)]
    public async Task<IActionResult> Employee(int id)
    {
        var employee = await employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
        {
            return RedirectToAction("PageNotFound", "Error");
        }

        var year = DateTimeOffset.Now.DateTime.ToClientTimeZone().Year;
        var model = new HospitalizationLeaveTransactionViewModel
        {
            EmployeeId = id,
            Nickname = employee.NickName,
            Year = year,
            IncludedRequestTrans = false,
            Transactions = await GetHospitalizationLeaveTransactionAsync(id, year, false)
        };

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.HospitalizationLeaveBalance)]
    public async Task<IActionResult> Employee(HospitalizationLeaveTransactionViewModel model)
    {
        var employee = await employeeService.GetEmployeeByIdAsync(model.EmployeeId);
        model.Nickname = employee.NickName;
        model.Transactions = await GetHospitalizationLeaveTransactionAsync(model.EmployeeId, model.Year, model.IncludedRequestTrans);

        return View(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.HospitalizationLeaveBalance)]
    public async Task<IActionResult> Create(int id)
    {
        var employee = await employeeService.GetEmployeeByIdAsync(id);
        var model = new HospitalizationLeaveTransaction
        {
            EmpId = id,
            Nickname = employee.NickName,
            AffectedFrom = DateTimeOffset.Now.DateTime.ToClientTimeZone()
        };
        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.HospitalizationLeaveBalance)]
    public async Task<IActionResult> Create(HospitalizationLeaveTransaction model)
    {
        if (ModelState.IsValid)
        {
            var criteria = new InsertTransactionCriteria
            {
                EmpId = model.EmpId,
                Description = model.Description,
                FromDate = model.AffectedFrom,
                DeductDays = model.DeductDays,
                CreditDays = model.CreditDays,
                CreatedDate = DateTimeOffset.Now.DateTime,
                CreatedBy = session.UserId
            };
            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            if (commandResult.IsSuccessful)
            {
                return RedirectToAction("Employee", new { id = model.EmpId });
            }

            ModelState.AddModelError(string.Empty, commandResult.Message);
        }

        return View(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.HospitalizationLeaveBalance)]
    public async Task<IActionResult> Edit(int id)
    {
        var criteria = new GetTransactionByIdCriteria
        {
            TransId = id
        };
        var transaction = await baseService.GetDataEntityAsync<HospitalizationLeaveTransaction>(criteria);
        if (transaction == null)
        {
            return RedirectToAction("PageNotFound", "Error");
        }

        return View(transaction);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.HospitalizationLeaveBalance)]
    public async Task<IActionResult> Edit(HospitalizationLeaveTransaction model)
    {
        var formCollection = Request.Form;
        if (formCollection["command"].Equals(Labels.Delete))
        {
            return await DeleteAsync(model);
        }

        return await EditAsync(model);
    }

    public async Task<IActionResult> ExportSummaryReport(HospitalizationLeaveBalanceViewModel model)
    {
        var fileName = $"HospitalizationLeaveSummary_{DateTimeOffset.Now.DateTime.ToClientTimeZone():yyyyMMdd}.xls";
        var criteria = new GetSummaryReportCriteria
        {
            Year = model.Year,
            EmployeeId = model.EmployeeId,
            TeamIds = model.TeamIds,
            IsLocal = model.IsLocal,
            SupervisorId = session.UserId,
            CompanyId = CurrentCompanyId
        };
        var employees = await baseService.GetDataAsync<HospitalizationLeaveBalanceSummaryReport>(criteria);
        var workbook = exportExcelService.ExportHospitalizationLeaveSummary(employees, model.Year);

        return ExcelFile(workbook, fileName);
    }

    private async Task<IEnumerable<HospitalizationLeaveTransaction>> GetHospitalizationLeaveTransactionAsync(int employeeId, int year, bool includedRequestTrans = true)
    {
        var criteria = new GetTransactionByEmployeeIdCriteria
        {
            EmployeeId = employeeId,
            IncludedRequestTrans = includedRequestTrans,
            Year = year
        };

        return await baseService.GetDataAsync<HospitalizationLeaveTransaction>(criteria);
    }

    private async Task<IActionResult> EditAsync(HospitalizationLeaveTransaction model)
    {
        var criteria = new UpdateTransactionCriteria
        {
            TransId = model.TransId,
            Description = model.Description,
            FromDate = model.AffectedFrom,
            DeductDays = model.DeductDays,
            CreditDays = model.CreditDays,
            ModifiedDate = DateTimeOffset.Now.DateTime,
            ModifiedBy = session.UserId
        };
        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        if (commandResult.IsSuccessful)
        {
            ViewBag.Updated = true;
        }
        else
        {
            ModelState.AddModelError(string.Empty, commandResult.Message);
        }

        return View(model);
    }

    private async Task<IActionResult> DeleteAsync(HospitalizationLeaveTransaction model)
    {
        var criteria = new DeleteTransactionCriteria
        {
            TransId = model.TransId,
            ModifiedDate = DateTimeOffset.Now.DateTime,
            ModifiedBy = session.UserId
        };
        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        if (commandResult.IsSuccessful)
        {
            return RedirectToAction("Employee", new { id = model.EmpId });
        }

        ModelState.AddModelError(string.Empty, commandResult.Message);
        return View(model);
    }
}