namespace HRM.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Filters;
using Core.Models;
using Core.Services;
using Criteria.AnnualLeave;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Models;
using Resources;
using Services.Interfaces;
using ViewModels.AnnualLeave;

public class AnnualLeaveBalanceController(
    AppSettings appSettings,
    IUserControlSession session,
    IEmployeeService employeeService,
    ITeamService teamService,
    IReportService reportService,
    IBaseService baseService) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
    public async Task<IActionResult> Index(
        string teamIds,
        string employee,
        int? year = null,
        int? employeeId = null,
        string employeeStatusIds = "1",
        int? category = null)
    {
        var isJoinedDateRule = session.Get<short>(SessionKey.AnnualLeaveRule) == (short)AnnualLeaveRuleType.JoinedDate;
        ViewBag.IsJoinedDateRule = isJoinedDateRule;
        ViewBag.HasPermissionToEdit = session.HasPermissionOnFunction(FunctionKey.AnnualLeaveBalance, Policy.Host);
        var model = new LeaveSummaryViewModel
        {
            Year = year ?? DateTimeOffset.Now.DateTime.ToClientTimeZone().Year,
            TeamIds = teamIds,
            Employee = employee,
            EmployeeId = employeeId ?? -1,
            Category = category,
            IsShowCloseBalance = appSettings.IsShowCloseBalance,
            EmployeeStatusIds = employeeStatusIds
        };
        return isJoinedDateRule 
            ? await GetJoinedDateRuleData("JoinedDateRule", model)
            : await GetYearlyRuleData("YearlyRule", model);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
    public async Task<IActionResult> GetYearlyRuleData(string view, LeaveSummaryViewModel model)
    {
        model.IsShowCloseBalance = appSettings.IsShowCloseBalance;
        model.LeaveSumaries = await GetAnnualLeaveBalanceAsync(model);

        return View(view, model);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
    public async Task<IActionResult> GetJoinedDateRuleData(string view, LeaveSummaryViewModel model)
    {
        model.IsShowCloseBalance = false;
        model.LeaveSumariesForJoinedDate = await GetAnnualLeaveBalanceForJoinedDateAsync(model);

        return View(view, model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
    public async Task<IActionResult> Adjust([FromBody] AnnualLeaveBalanceMultiViewModel model)
    {
        var criteria = new AdjustEmployeeAnnualLeaveCriteria
        {
            CompanyId = CurrentCompanyId,
            EmployeeIds = model.EmpIds,
            Description = model.Description,
            CreditDays = model.CreditDays,
            DeductDays = model.DeductDays,
            FromDate = model.AffectedFrom,
            ActorId = EmployeeSession.EmpId
        };

        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return Json(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
    public async Task<IActionResult> Reset(int minBalance, string descriptionReset)
    {
        var date = new DateTimeOffset(DateTimeOffset.Now.Year, 1, 1, 0, 0, 0, TimeSpan.Zero).AddDays(-1).DateTime;
        var teams = await teamService.GetBySupervisorAsync(EmployeeSession.EmpId);
        var criteria = new GetAnnualLeaveBalanceCriteria
        {
            Year = date.Year,
            EmpId = null,
            TeamIds = string.Join(",", teams.Select(x => x.Id)),
            IsLocal = null,
            CompanyId = CurrentCompanyId
        };
        var entityList = await reportService.GetAnnualLeaveBalanceReportAsync(criteria);
        var resetAnnualLeaveCriteria = new ResetAnnualLeaveCriteria
        {
            ActorId = EmployeeSession.EmpId,
            Description = descriptionReset,
            AnnualLeaves = entityList.Where(m => m.LeaveBalance > minBalance).ToList(),
            FromDate = date,
            ResetDays = minBalance
        };
        var result = await baseService.ExecuteNonQueryAsync(resetAnnualLeaveCriteria);

        return Json(result);
    }

    [HrmPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
    public async Task<IActionResult> Employee(int id)
    {
        ViewBag.IsJoinedDateRule = session.Get<short>(SessionKey.AnnualLeaveRule) == (short)AnnualLeaveRuleType.JoinedDate;
        var fromDate = DateTimeOffset.Now.DateTime.ToClientTimeZone();
        var model = new AnnualLeaveBalanceViewModel
        {
            EmployeeId = id,
            Year = fromDate.Year,
            ExcludedRequestTrans = true,
            AnnualLeaveBalances = await GetAnnualLeaveTransactionAsync(id, false, fromDate),
            Employee = await employeeService.GetEmployeeByIdAsync(id)
        };

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
    public async Task<IActionResult> Employee(AnnualLeaveBalanceViewModel model)
    {
        ViewBag.IsJoinedDateRule = session.Get<short>(SessionKey.AnnualLeaveRule) == (short)AnnualLeaveRuleType.JoinedDate;
        var fromDate = model.Year == 0
            ? DateTimeOffset.Now.DateTime.ToClientTimeZone()
            : new DateTimeOffset(model.Year, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
        model.Employee = await employeeService.GetEmployeeByIdAsync(model.EmployeeId);
        model.AnnualLeaveBalances = await GetAnnualLeaveTransactionAsync(model.EmployeeId, !model.ExcludedRequestTrans, fromDate);
        return View(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await employeeService.GetAnnualLeaveTransactionAsync(id);
        ViewBag.Employee = await employeeService.GetEmployeeByIdAsync(model.EmpId);
        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
    public async Task<IActionResult> Edit(AnnualLeaveTransactionViewModel model)
    {
        var formCollection = Request.Form;
        ViewBag.Employee = await employeeService.GetEmployeeByIdAsync(model.EmpId);
        if (formCollection["command"].Equals(Labels.Delete))
        {
            return await DeleteAsync(model);
        }

        return await EditAsync(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
    public async Task<IActionResult> Create(int id)
    {
        ViewBag.Employee = await employeeService.GetEmployeeByIdAsync(id);
        var model = new AnnualLeaveTransactionViewModel
        {
            EmpId = id,
            AffectedFrom = DateTimeOffset.Now.DateTime.ToClientTimeZone()
        };
        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
    public async Task<IActionResult> Create(AnnualLeaveTransactionViewModel model)
    {
        ViewBag.Employee = await employeeService.GetEmployeeByIdAsync(model.EmpId);
        if (ModelState.IsValid)
        {
            var criteria = new InsertAnnualLeaveTransactionCriteria
            {
                EmpId = model.EmpId,
                RequestId = null,
                Description = model.Description,
                FromDate = model.AffectedFrom,
                DeductDays = model.DeductDays,
                CreditDays = model.CreditDays,
                CreatedDate = DateTimeOffset.Now.DateTime,
                CreatedBy = EmployeeSession.EmpId
            };
            var result = await baseService.ExecuteNonQueryAsync(criteria);

            if (result.IsSuccessful)
            {
                return RedirectToAction("Employee", "AnnualLeaveBalance", new { id = model.EmpId });
            }

            ModelState.AddModelError(string.Empty, result.Message);
        }

        return View(model);
    }

    private async Task<IEnumerable<LeaveSummary>> GetAnnualLeaveBalanceAsync(LeaveSummaryViewModel model)
    {
        var criteria = new GetAnnualLeaveBalanceCriteria
        {
            Year = model.Year,
            EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
            TeamIds = model.TeamIds,
            IsLocal = (byte?)model.Category,
            CompanyId = CurrentCompanyId,
            EmployeeStatusIds = model.EmployeeStatusIds
        };
        return await reportService.GetAnnualLeaveBalanceReportAsync(criteria);
    }

    private async Task<IEnumerable<LeaveSummaryForJoinedDate>> GetAnnualLeaveBalanceForJoinedDateAsync(LeaveSummaryViewModel model)
    {
        var criteria = new GetAnnualLeaveBalanceForJoinedDateCriteria
        {
            FromDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
            EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
            TeamIds = model.TeamIds,
            IsLocal = (byte?)model.Category,
            CompanyId = CurrentCompanyId,
            EmployeeStatusIds = model.EmployeeStatusIds
        };
        return await reportService.GetAnnualLeaveBalanceForJoinedDateReportAsync(criteria);
    }

    private async Task<IEnumerable<AnnualLeaveTransactionViewModel>> GetAnnualLeaveTransactionAsync(int empId, bool excludedRequest, DateTime fromDate)
    {
        var criteria = new GetAnnualLeaveTransactionCriteria
        {
            EmpId = empId,
            ExcludedRequestTrans = excludedRequest,
            FromDate = fromDate
        };

        return await employeeService.GetAnnualLeaveTransactionAsync(criteria);
    }

    private async Task<IActionResult> EditAsync(AnnualLeaveTransactionViewModel model)
    {
        if (ModelState.IsValid)
        {
            var criteria = new UpdateAnnualLeaveTransactionCriteria
            {
                LeaveTransId = model.LeaveTransId,
                Description = model.Description,
                FromDate = model.AffectedFrom,
                DeductDays = model.DeductDays,
                CreditDays = model.CreditDays,
                ModifiedDate = DateTimeOffset.Now.DateTime,
                ModifiedBy = EmployeeSession.EmpId
            };
            var result = await baseService.ExecuteNonQueryAsync(criteria);

            if (result.IsSuccessful)
            {
                ViewBag.Updated = true;
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
        }

        return View(model);
    }

    private async Task<IActionResult> DeleteAsync(AnnualLeaveTransactionViewModel model)
    {
        var criteria = new DeleteAnnualLeaveTransactionCriteria
        {
            LeaveTransId = model.LeaveTransId,
            ModifiedDate = DateTimeOffset.Now.DateTime,
            ModifiedBy = EmployeeSession.EmpId
        };
        var result = await baseService.ExecuteNonQueryAsync(criteria);

        if (result.IsSuccessful)
        {
            return RedirectToAction("Employee", "AnnualLeaveBalance", new { id = model.EmpId });
        }

        ModelState.AddModelError(string.Empty, result.Message);
        return View(model);
    }
}