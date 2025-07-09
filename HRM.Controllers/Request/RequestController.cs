using HRM.Models;

namespace HRM.Controllers.Request;

using System;
using System.Collections.Generic;
using System.Linq;
using Repositories.Interfaces;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Enums;
using Core.Filters;
using Core.Models;
using Criteria.Supervisor;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Criteria.Request;
using HRM.Models.Request;
using HRM.Services.Interfaces.Request;
using HRM.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;
using Models.Supervisor;
using Fanex.Common.Extensions;
using Services.Implement;
using Services.Interfaces;
using HRM.Core.Services;

public class RequestController(
    IUserControlSession session,
    IEmployeeService employeeService,
    IRequestService requestService,
    ICommonDataService commonDataService,
    ITeamService teamService,
    IAttachFileService attachFileService,
    IBaseService baseService,
    IRequestRepository requestRepository,
    ICompanyService companyService) : BaseController(session)
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
        var isLocal = request.IsLocal != 0;
        if (isLocal && request.IsShowCrossCompany)
        {
            var nexRequest = (await NexRequests(request)).ToList();
            request.Requests = nexRequest;
        }

        await GetRequestAsync(request);
        await GetInfoAttachFileAsync(request.Requests);
        ViewBag.NexCompanyIds = Constants.NexCompanyIds;
        return View(EmployeeRequestViewName, request);
    }

    private async Task<IEnumerable<RequestEntity>> NexRequests(RequestViewModel model)
    {
        List<RequestEntity> responseRequests = [];
        var status = requestRepository.GetNexRequestStatus(model.StatusIds);
        var requestType = requestRepository.GetNexRequestType(model.RequestType);
        int? leaveType = null;
        if (model.LeaveType != null)
        {
            leaveType = requestRepository.GetNexLeaveType(model.LeaveType.Value);
        }
        var supervisorCompanyCriteria = new GetSupervisorCompaniesCriteria
        {
            EmpId = session.UserId
        };
        var supervisorCompanyIds = await baseService.GetDataAsync<int>(supervisorCompanyCriteria);

        var companies = await companyService.GetAllCompany();

        var nexCompanies = supervisorCompanyIds
            .Where(s => Constants.NexCompanyIds.Contains(s));
        if (!nexCompanies.Any())
        {
            return responseRequests;
        }
        const int AssistantLeader = 1;
        List<NexRequestModel> requests = [];
        foreach (var companyId in nexCompanies)
        {
            var criteria = new GetNexRequestCriteria
            {
                EmpId = model.EmployeeId == -1 ? null : model.EmployeeId,
                FromDate = model.FromDate.ToStartOfDay(),
                ToDate = model.ToDate.ToEndOfDay(),
                Status = status,
                RequestType = requestType,
                isFilterByEffectedDate = model.IsFilterByAffectedDate,
                ActorId = session.UserId,
                LeaveTypeId = leaveType,
                Role = AssistantLeader,
                ExcludedEmpID = session.UserId,
                CompanyId = companyId
            };

            var responseData = await baseService.GetDataAsync<NexRequestModel>(criteria);
            var mappedData = responseData.Select(item =>
            {
                item.CompanyId = companyId;
                item.Status = (byte)requestRepository.GetGroupByNexRequestStatus(item.Status);
                return item;
            });
            requests.AddRange(mappedData);
        }

        Parallel.ForEach(requests, r =>
        {
            var companyName = companies
                .Where(c => c.CompanyId == r.CompanyId)
                .Select(c => c.Name)
                .FirstOrDefault();
            r.Requester += $" ({companyName})";
        });

        responseRequests = requestRepository
            .MappingNexRequestToGroupRequest(requests, session.UserId)
            .OrderByDescending(item => item.CreatedDate)
            .ToList();

        return responseRequests;
    }

    public async Task<ActionResult> ViewHistory(string id)
    {
        var splitCustomId = id.Split("_");
        var companyId = int.Parse(splitCustomId[1]);
        var requestId = int.Parse(splitCustomId[0]);
        var criteria = new GetNexRequestHistoryCriteria
        {
            RequestId = requestId,
            CompanyId = companyId
        };
        var history = await baseService.GetDataAsync<NexRequestModel>(criteria);
        Parallel.ForEach(history, h =>
        {
            h.Status = (byte)requestRepository.GetGroupByNexRequestStatus(h.Status);
        });
        var requestType = history.FirstOrDefault()?.RequestType;
        var isCheckInOutType = requestType is (int)RequestType.CheckIn or (int)RequestType.CheckOut;
        return PartialView(isCheckInOutType ? "NexHistory/_CheckInOutHistory" : "NexHistory/_LeaveHistory", history);
    }

    private async Task GetInfoAttachFileAsync(IList<RequestEntity> requests)
    {
        foreach (var request in requests)
        {
            request.AttachFiles = await attachFileService.GetInfoAttachFileAsync(request.Id, $"{request.CompanyId}/requests");
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
        CommandResult commandResult;
        if (Constants.NexCompanyIds.Contains(model.RequesterCompanyId))
        {
            commandResult = await requestService.SendNexHrmProcessRequest(session.UserId, model);
        }
        else
        {
            commandResult = model.Command switch
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
        }

        return new JsonResult(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ApproveRequest)]
    public async Task<IActionResult> ProcessMultipleRequests(ApproveRequestsParam model)
    {
        model.ActorId = session.UserId;
        var nexCompanyRequests = model.Requests.Where(r => Constants.NexCompanyIds.Contains(r.RequesterCompanyId)).ToList();
        var localRequests = model.Requests.Where(r => !Constants.NexCompanyIds.Contains(r.RequesterCompanyId)).ToList();

        var commandResults = new List<CommandResult>();
        if (nexCompanyRequests.Count > 0)
        {
            model.Requests = nexCompanyRequests;
            var nexCommandResults = await requestService.SendNexHrmMultipleProcessRequest(session.UserId, model);
            commandResults.AddRange(nexCommandResults);
        }
        model.Requests = localRequests;
        var localCommandResult = await requestService.ApproveRequests(model);
        commandResults.Add(localCommandResult);
        if (localCommandResult.IsSuccessful)
        {
            await requestService.SendMailProcessRequests(model, EmployeeSession, model.Command);
        }

        var finalResult = commandResults.Find(r => !r.IsSuccessful) ?? commandResults.Find(r => true);

        return new JsonResult(finalResult);
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
                // Handle other request types
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
            ToDate = request.IsFilterByAffectedDate ? request.ToDate.ToEndOfDay() : request.ToDate.ToEndOfDay().ToServerTimeZone(),
            TeamIds = request.TeamIds,
            Status = request.StatusIds,
            RequestType = request.RequestType,
            IsQueryByCreatedDate = !request.IsFilterByAffectedDate,
            RequestViewType = request.ViewType,
            SupervisorId = session.UserId,
            IsLocal = request.IsLocal.HasValue ? request.IsLocal.Value == 1 : null,
            LeaveTypeId = request.LeaveType,
            CompanyId = CurrentCompanyId,
            IsShowCrossCompany = request.IsShowCrossCompany
        };

        var requests = await requestService.GetRequestAsync(criteria);

        Parallel.ForEach(requests, rq =>
        {
            rq.FromTime = rq.FromTime.ToClientTimeZone();
            rq.ToTime = rq.ToTime.ToClientTimeZone();
        });
        request.Requests ??= new List<RequestEntity>();
        requests.ForEach(item =>
        {
            request.Requests.Add(item);
        });
        request.CurrentUserId = session.UserId;
        request.Requests = request.Requests
            .OrderByDescending(r => r.CreatedDate)
            .ToList();
        request.Requests = request.ViewType == RequestViewType.PersonalSpace ? SortRequest(request.Requests) : request.Requests;
    }

    private List<RequestEntity> SortRequest(IList<RequestEntity> requests)
    {
        return requests.OrderByDescending(SortRequestValidate).ToList();
    }

    private bool SortRequestValidate(RequestEntity t)
    {
        var pending = t.Status.Id == (byte)CurrentRequestStatus.Pending
                      && (t.ApproverId == session.UserId || t.VerifierId == session.UserId);
        var verified = t.Status.Id == (byte)CurrentRequestStatus.Verified && t.ApproverId == session.UserId;
        return pending || verified;
    }
}