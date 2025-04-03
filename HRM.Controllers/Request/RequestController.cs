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
using HRM.Criteria.Request;
using HRM.Models.Request;
using HRM.Services.Interfaces.Request;
using HRM.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;
using Models.Supervisor;
using Services.Implement;
using Services.Interfaces;

public class RequestController(
    IUserControlSession session,
    IEmployeeService employeeService,
    IRequestService requestService,
    ICommonDataService commonDataService,
    ITeamService teamService,
    IAttachFileService attachFileService) : BaseController(session)
{
    private const string EmployeeRequestViewName = "EmployeeRequest";

    [HrmPermission(Policy.Host, FunctionKey.VerifyRequest, FunctionKey.ApproveRequest)]
    public async Task<IActionResult> Member(RequestViewModel request)
    {
        request.ViewType = RequestViewType.PersonalSpace;
        var team = await teamService.GetAllAsync();
        request.TeamIds ??= string.Join(",", team.Select(x => x.Id));

        if (request.StatusId == Constants.DefaultRequestFilterStatusId)
        {
            const long fromDays = -180;
            request.StatusId = Constants.PendingAndVerifiedStatusId;
            request.FromDate = DateTimeOffset.Now.Date.AddDays(fromDays);
            request.IsFilterByAffectedDate = false;
        }
        await GetRequestAsync(request);
        await GetInfoAttachFileAsync(request.Requests);
        return View(EmployeeRequestViewName, request);
    }

    private async Task GetInfoAttachFileAsync(IList<RequestEntity> requests)
    {
        foreach (var request in requests)
        {
            request.AttachFiles = await attachFileService.GetInfoAttachFileAsync(request.Id, $"{CurrentCompanyId}/requests");
        }
    }

    [HrmPermission(Policy.Host, FunctionKey.ProcessRequest)]
    public async Task<ViewResult> HRView(RequestViewModel request)
    {
        request.StatusId = request.StatusId == Constants.DefaultRequestFilterStatusId ? 0 : request.StatusId;
        request.ViewType = RequestViewType.HRSpace;
        var team = await teamService.GetAllAsync();
        request.TeamIds ??= string.Join(",", team.Select(x => x.Id));
        await GetRequestAsync(request);
        await GetInfoAttachFileAsync(request.Requests);

        return View(EmployeeRequestViewName, request);
    }

    [HrmPermission(Policy.Read, FunctionKey.DailyRequest)]
    public async Task<ViewResult> DailyView(RequestViewModel request)
    {
        request.StatusId = request.StatusId == Constants.DefaultRequestFilterStatusId ? 0 : request.StatusId;
        request.ViewType = RequestViewType.CompanySpace;
        var team = await teamService.GetAllAsync();
        request.TeamIds ??= string.Join(",", team.Select(x => x.Id));

        await GetRequestAsync(request);

        return View(EmployeeRequestViewName, request);
    }

    [HrmPermission(Policy.Host, FunctionKey.VoidRequest)]
    public async Task<ActionResult> VoidRequest(RequestViewModel request)
    {
        request.StatusId = request.StatusId == Constants.DefaultRequestFilterStatusId ? 0 : request.StatusId;
        request.ViewType = RequestViewType.SASpace;
        var teams = await teamService.GetAllAsync();
        request.TeamIds ??= string.Join(",", teams.Select(x => x.Id));
        await GetRequestAsync(request);
        await GetInfoAttachFileAsync(request.Requests);

        return View(EmployeeRequestViewName, request);
    }

    public async Task<IActionResult> GetEmployeesByActingUser(byte role, string query = "", byte? requestType = null)
    {
        var criteria = new GetEmployeeSupervisorByQueryCriteria
        {
            CompanyId = CurrentCompanyId,
            EmpId = session.UserId,
            Role = role,
            RequestType = requestType,
            Query = query.Trim(),
            IncludeLoginUser = true
        };
        var supervisors = await employeeService.GetSupervisorByQueryAsync(criteria);

        return Json(supervisors);
    }

    public async Task<IActionResult> GetVerifiers(int employeeId, byte requestType)
    {
        var verifierList = await GetSupervisorByRequestType(employeeId, (byte)RoleSystem.Verifier, requestType);

        return Json(verifierList);
    }

    public async Task<IActionResult> GetApprovers(int employeeId, byte requestType)
    {
        var approverList = await GetSupervisorByRequestType(employeeId, (byte)RoleSystem.Approver, requestType);

        return Json(approverList);
    }

    [NonAction]
    public async Task<IEnumerable<SupervisorNameModel>> GetSupervisorByRequestType(int employeeId, byte role, byte requestType)
    {
        var criteria = new GetSupervisorByRequestTypeCriteria
        {
            EmpId = employeeId,
            Role = role,
            RequestType = requestType
        };
        return await employeeService.GetSupervisorByRequestTypeAsync(criteria);
    }

    [HrmPermission(Policy.Host, FunctionKey.VerifyRequest, FunctionKey.ApproveRequest, FunctionKey.ProcessRequest, FunctionKey.VoidRequest)]
    public async Task<PartialViewResult> PartialViewProcessRequest(ProcessRequestViewModel model)
    {
        if (model is { RequestId: > 0, RoleId: (byte)RoleSystem.Approver, Command: RequestCommand.Approve })
        {
            await HandleProcessRequest(model);
        }

        return PartialView("_ProcessRequest", model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.VerifyRequest, FunctionKey.ApproveRequest, FunctionKey.ProcessRequest, FunctionKey.VoidRequest)]
    public async Task<IActionResult> ProcessRequest(ProcessRequestParam model)
    {
        var commandResult = model.Command switch
        {
            RequestCommand.Accept or RequestCommand.Approve or RequestCommand.Process => await requestService
                .ApproveRequest(session.UserId, model),
            RequestCommand.Reset => await requestService.ResetRequest(session.UserId, model),
            RequestCommand.NotAccept or RequestCommand.Reject or RequestCommand.Void => await requestService
                .RejectRequest(session.UserId, model),
            _ => await requestService.VoidRequest(session.UserId, model)
        };

        if (commandResult.IsSuccessful)
        {
            await requestService.SendMailProcessRequest(model, EmployeeSession, model.Command);
        }

        return new JsonResult(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ApproveRequest)]
    public async Task<IActionResult> ProcessMultipleRequests(ApproveRequestsParam model)
    {
        model.ActorId = session.UserId;

        var commandResult = await requestService.ApproveRequests(model);

        if (commandResult.IsSuccessful)
        {
            await requestService.SendMailProcessRequests(model, EmployeeSession, model.Command);
        }

        return new JsonResult(commandResult);
    }

    private async Task HandleProcessRequest(ProcessRequestViewModel model)
    {
        switch (model.RequestTypeId)
        {
            case (byte)RequestType.CheckIn:
            case (byte)RequestType.CheckOut:
                model.CheckInOutDetail = await requestService.GetCheckInOutDetailToReviewDiscipline(model.RequestId);
                break;

            case (byte)RequestType.LateIn:
            case (byte)RequestType.EarlyOut:
                model.LateInEarlyOutDetail = await requestService.GetLateInEarlyOutDetailToReviewDiscipline(model.RequestId);
                break;

            case (byte)RequestType.ChangeShift:
                model.ChangeShiftDetails = await requestService.GetChangeShiftDetailToReviewDiscipline(model.RequestId);
                break;

            case (byte)RequestType.Leave:
                model.LeaveDisciplines = await requestService.GetLeaveRequestDetailToReviewDiscipline(model.RequestId);
                break;

            default:
                break;
        }
    }

    private async Task GetRequestAsync(RequestViewModel request)
    {
        request.LeaveTypes = await commonDataService.GetLeaveTypeAsync();
        int? employeeId = null;
        if (request.EmployeeId != -1 && !string.IsNullOrWhiteSpace(request.Employee))
        {
            employeeId = request.EmployeeId;
        }

        var criteria = new GetRequestCriteria
        {
            EmpId = employeeId,
            FromDate = request.IsFilterByAffectedDate ? request.FromDate.ToStartOfDay() : request.FromDate.ToStartOfDay().ToServerTimeZone(),
            ToDate = request.IsFilterByAffectedDate ? request.ToDate.ToEndOfDay() : request.ToDate.ToEndOfDay().ToClientTimeZone(),
            TeamIds = request.TeamIds,
            Status = request.StatusIds,
            RequestType = request.RequestType,
            IsQueryByCreatedDate = !request.IsFilterByAffectedDate,
            RequestViewType = request.ViewType,
            SupervisorId = session.UserId,
            IsLocal = request.IsLocal.HasValue ? request.IsLocal.Value == 1 : null,
            LeaveTypeId = request.LeaveType,
            CompanyId = CurrentCompanyId
        };

        var requests = await requestService.GetRequestAsync(criteria);

        Parallel.ForEach(requests, rq =>
        {
            rq.FromTime = rq.FromTime.ToClientTimeZone();
            rq.ToTime = rq.ToTime.ToClientTimeZone();
        });
        request.CurrentUserId = session.UserId;
        request.Requests = request.ViewType == RequestViewType.PersonalSpace ? SortRequest(requests) : requests;
    }

    private List<RequestEntity> SortRequest(IList<RequestEntity> requests)
    {
        return requests.OrderByDescending(SortRequestValidate).ToList();
    }

    private bool SortRequestValidate(RequestEntity t)
    {
        var pending = t.Status.Id == (byte)CurrentRequestStatus.Pending
                      && (t.ApprovedBy.Id == session.UserId || t.VerifiedBy.Id == session.UserId);
        var verified = t.Status.Id == (byte)CurrentRequestStatus.Verified && t.ApprovedBy.Id == session.UserId;
        return pending || verified;
    }
}