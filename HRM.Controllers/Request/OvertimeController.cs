namespace HRM.Controllers.Request;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Enums;
using Core.Filters;
using Criteria.Supervisor;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Services.Interfaces.Request;
using Microsoft.AspNetCore.Mvc;
using Models.Overtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Resources;
using Services.ExportExcel;
using Services.Interfaces;
using ViewModels.Overtime;

public class OvertimeController(
    IUserControlSession session,
    IEmployeeService employeeService,
    IRequestService requestService,
    IOvertimeRequestService overtimeService,
    IExportOvertimeService exportOvertimeService,
    ITeamService teamService) : BaseController(session)
{
    [HttpGet]
    public async Task<IActionResult> EmployeeView(MonthlyOvertimeRequestViewModel model)
    {
        model.Employee ??= EmployeeSession.DisplayName;
        model.EmployeeId = model.EmployeeId <= 0 ? session.UserId : model.EmployeeId;
        model.OvertimeRequests = await overtimeService.GetMonthlyRequestByEmployeeId(model.EmployeeId, model.Year);

        Parallel.ForEach(model.OvertimeRequests, overTimeRequest =>
        {
            overTimeRequest.ApprovedDate = overTimeRequest.ApprovedDate?.ToClientTimeZone();
            overTimeRequest.VerifiedDate = overTimeRequest.VerifiedDate?.ToClientTimeZone();
        });

        return View(model);
    }

    public async Task<IActionResult> Details(int employeeId, DateTime month)
    {
        var monthlyRequest = await overtimeService.GetMonthlyRequestDetailsByEmployeeId(employeeId, month);

        return View(monthlyRequest);
    }

    public async Task<IActionResult> Edit(int employeeId, DateTime month)
    {
        var monthlyRequest = await overtimeService.GetMonthlyRequestDetailsByEmployeeId(employeeId, month);
        var supervisor = await requestService.GetVerifierAndApproverAsync(employeeId, RequestType.Overtime);

        var model = new EditMonthlyRequestViewModel
        {
            MonthlyRequest = monthlyRequest,
            VerifierId = monthlyRequest.Verifier?.EmpId ?? 0,
            ApproverId = monthlyRequest.Approver?.EmpId ?? 0,
            Verifiers = supervisor.Verifiers,
            Approvers = supervisor.Approvers,
            AutoApproveRequest = EmployeeSession.AutoApproveRequest
        };

        model.Verifiers = await employeeService.CheckAndAddMissingSupervisor(model.Verifiers.ToList(), model.VerifierId);
        model.Approvers = await employeeService.CheckAndAddMissingSupervisor(model.Approvers.ToList(), model.ApproverId);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditMonthlyOvertimeRequest model)
    {
        var commandResult = await overtimeService.EditMonthlyRequest(session.UserId, model);
        return Json(commandResult);
    }

    [HrmPermission(Policy.Host, FunctionKey.VerifyOvertime, FunctionKey.ApproveOvertime)]
    public async Task<IActionResult> MemberOvertime(MemberMonthlyOvertimeRequestViewModel model)
    {
        var teams = await teamService.GetAllAsync();
        model.IsShowCrossCompany = model.TeamIds == null || model.IsShowCrossCompany;
        model.TeamIds ??= string.Join(",", teams.Select(x => x.Id));
        model.MemberRequests = await GetMemberOvertimeRequestAsync(model);
        ViewBag.CurrentUserId = session.UserId;
        ViewBag.IsVerifier = session.HasPermissionOnFunction(FunctionKey.VerifyOvertime, Policy.Host);
        ViewBag.IsApprover = session.HasPermissionOnFunction(FunctionKey.ApproveOvertime, Policy.Host);

        return View(model);
    }

    private async Task<IEnumerable<MonthlyOvertimeRequest>> GetMemberOvertimeRequestAsync(MemberMonthlyOvertimeRequestViewModel model)
    {
        var requestStatusIds = Enum.GetValues<CurrentRequestStatus>().Cast<int>().Where(x => x > 0);
        var statusIds = model.StatusId switch
        {
            -1 => $"{(byte)CurrentRequestStatus.Pending},{(byte)CurrentRequestStatus.Verified}",
            0 => string.Join(",", requestStatusIds),
            _ => model.StatusId.ToString()
        };

        var param = new GetMonthlyOvertimeRequestParam
        {
            ActorId = session.UserId,
            Month = model.Month,
            TeamIds = model.TeamIds,
            EmployeeId = model.EmployeeId,
            StatusIds = statusIds,
            IsLocal = model.IsLocal,
            CompanyId = CurrentCompanyId,
            IsShowCrossCompany = model.IsShowCrossCompany
        };
        model.MemberRequests = await overtimeService.GetMemberMonthlyRequestAsync(param);

        return model.MemberRequests;
    }

    public async Task<IActionResult> MemberDetails(long id)
    {
        var monthlyRequest = await overtimeService.GetMonthlyRequestDetailsById(id);
        ViewBag.CurrentUserId = session.UserId;
        return View(monthlyRequest);
    }

    [HrmPermission(Policy.Host, FunctionKey.ProcessOvertime)]
    public async Task<IActionResult> EmployeeOvertime(MemberMonthlyOvertimeRequestViewModel model)
    {
        var teams = await teamService.GetAllAsync();
        model.TeamIds ??= string.Join(",", teams.Select(x => x.Id));

        return View(model);
    }

    public async Task<IActionResult> ListEmployeeOvertime(MemberMonthlyOvertimeRequestViewModel model)
    {
        model.MemberRequests = await GetEmployeeOvertimeRequestAsync(model);

        return PartialView("_ListEmployeeOvertime", model);
    }

    private async Task<IEnumerable<MonthlyOvertimeRequest>> GetEmployeeOvertimeRequestAsync(MemberMonthlyOvertimeRequestViewModel model)
    {
        var requestStatusIds = Enum.GetValues<CurrentRequestStatus>().Cast<int>().Where(x => x > 0);
        var statusIds = model.StatusId switch
        {
            -1 => $"{(byte)CurrentRequestStatus.Pending},{(byte)CurrentRequestStatus.Verified}",
            0 => string.Join(",", requestStatusIds),
            _ => model.StatusId.ToString()
        };

        var param = new GetMonthlyOvertimeRequestParam
        {
            ActorId = session.UserId,
            Month = model.Month,
            TeamIds = model.TeamIds,
            EmployeeId = model.EmployeeId,
            StatusIds = statusIds,
            IsProcessor = true,
            OvertimeType = model.OvertimeType,
            IsLocal = model.IsLocal,
            CompanyId = CurrentCompanyId
        };
        model.MemberRequests = await overtimeService.GetMemberMonthlyRequestAsync(param);

        return model.MemberRequests;
    }

    public async Task<IActionResult> EmployeeDetails(long id)
    {
        var monthlyRequest = await overtimeService.GetMonthlyRequestDetailsById(id);
        return View(monthlyRequest);
    }

    public async Task<IActionResult> GetActingEmployees(string query)
    {
        var criteria = new GetEmployeeSupervisorByQueryCriteria
        {
            CompanyId = CurrentCompanyId,
            EmpId = session.UserId,
            Role = (byte)RoleSystem.OnBehalfRequest,
            RequestType = (byte)RequestType.Overtime,
            Query = string.IsNullOrWhiteSpace(query) ? string.Empty : query.Trim(),
            IncludeLoginUser = true
        };

        var employees = await employeeService.GetSupervisorByQueryAsync(criteria);
        return Json(employees);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.VerifyOvertime, FunctionKey.ApproveOvertime, FunctionKey.ProcessOvertime)]
    public async Task<IActionResult> EditMonthlyRequestStatus(string requestIds, RequestCommand command, byte roleId)
    {
        var commandResult = await overtimeService.EditMonthlyRequestStatus(session.UserId, requestIds, command, roleId);
        return Json(commandResult, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver()
        });
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.ProcessOvertime)]
    public async Task<IActionResult> CheckInvalidOvertime(string requestIds)
    {
        var isInvalid = await overtimeService.CheckInvalidOvertimes(requestIds);

        return Json(isInvalid);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ProcessOvertime)]
    public async Task ReminderClaimOvertime(string subject, string content)
    {
        await Task.Run(() => overtimeService.SendMailToReminderClaimOvertime(subject, content));
    }

    public async Task<IActionResult> Export(MemberMonthlyOvertimeRequestViewModel model)
    {
        var fileName = $"{Labels.EmployeeOvertimeReport}.xlsx";
        const bool isMemberOvertime = false;
        model.MemberRequests = await GetEmployeeOvertimeRequestAsync(model);
        var exportData = exportOvertimeService.Export(model.MemberRequests, isMemberOvertime);

        return File(exportData, MimeType.XLSX, fileName);
    }

    public async Task<IActionResult> ExportMemberOvertime(MemberMonthlyOvertimeRequestViewModel model)
    {
        var fileName = $"{Labels.MemberOvertimeReport}.xlsx";
        const bool isMemberOvertime = true;

        model.MemberRequests = await GetMemberOvertimeRequestAsync(model);
        var exportData = exportOvertimeService.Export(model.MemberRequests, isMemberOvertime);

        return File(exportData, MimeType.XLSX, fileName);
    }
}