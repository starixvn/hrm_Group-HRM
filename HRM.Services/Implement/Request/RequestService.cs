using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Core.XMLExtensions;
using HRM.Criteria.Request;
using HRM.Criteria.Request.ChangeShift;
using HRM.Criteria.Request.CheckInOut;
using HRM.Criteria.Request.LateInEarlyOut;
using HRM.Criteria.Request.Leave;
using HRM.Models.Request;
using HRM.Models.Request.ChangeShift;
using HRM.Repositories.Interfaces;
using HRM.Resources;
using HRM.Services.Interfaces;
using HRM.Services.Interfaces.Request;
using HRM.ViewModels.Request;
using IEmployeeService = HRM.Services.Interfaces.IEmployeeService;

namespace HRM.Services.Implement.Request;

#pragma warning disable S107 // Methods should not have too many parameters
public class RequestService(
    IHttpClientFactory clientFactory,
    IRequestRepository requestRepository,
    IBaseService baseService,
    ICheckInOutRequestService checkInOutRequestService,
    ILateInEarlyOutRequestService lateInEarlyOutRequestService,
    IChangeShiftRequestService changeShiftRequestService,
    ILeaveRequestService leaveRequestService,
    IKeepInViewRequestService keepInViewRequestService,
    IUserControlSession session,
    IEmployeeService employeeService,
    ICompanyService companyService,
    NexConfig nexConfig) : IRequestService
#pragma warning restore S107 // Methods should not have too many parameters
{
    public static string ToRequestMessage(RequestEntity request, bool isNoFormat = false)
    {
        var message = request.RequestType.Id switch
        {
            (byte)RequestType.Leave => GetMessageForLeave(request, isNoFormat),
            (byte)RequestType.ChangeShift => GetMessageForChangeShift(request, isNoFormat),
            (byte)RequestType.CheckIn or (byte)RequestType.CheckOut => GetMessageForCheckInOut(request, isNoFormat),
            (byte)RequestType.LateIn => GetMessageForLateIn(request, isNoFormat),
            (byte)RequestType.EarlyOut => GetMessageForEarlyOut(request, isNoFormat),
            (byte)RequestType.KeepInView => GetMessageForKeepInView(request, isNoFormat),
            _ => string.Empty
        };

        return message;
    }

    public static string GetReasonRequest(short requestTypeId, byte reasonId)
    {
        return requestTypeId is (byte)RequestType.CheckIn or (byte)RequestType.CheckOut
            ? GetCheckInOutReason(reasonId)
            : GetLateInEarlyOutReason(reasonId);
    }

    private static string GetCheckInOutReason(byte reasonId)
    {
        return reasonId switch
        {
            (byte)CheckInOutReason.VanLate => Labels.VanLate,
            (byte)CheckInOutReason.BusinessTrip => Labels.BusinessTrip,
            (byte)CheckInOutReason.Forgot => Labels.Forgot,
            (byte)CheckInOutReason.Other => Labels.Other,
            _ => string.Empty
        };
    }

    private static string GetLateInEarlyOutReason(byte reasonId)
    {
        return reasonId switch
        {
            (byte)LateInEarlyOutReason.ManagementAllow => Labels.ManagementAllow,
            (byte)LateInEarlyOutReason.Other => Labels.Other,
            _ => string.Empty
        };
    }

    public async Task<IList<RequestEntity>> GetRequestAsync(GetRequestCriteria criteria)
    {
        return await requestRepository.GetRequestAsync(criteria);
    }

    public async Task<VerifierAndApproverForRequest> GetVerifierAndApproverAsync(int employeeId, RequestType requestType)
    {
        var companyId = session.Get<int>(SessionKey.CurrentCompanyId);
        var criteria = new GetVerifierAndApproverCriteria
        {
            EmpId = employeeId,
            RequestType = (byte)requestType,
            CompanyId = companyId
        };
        return await requestRepository.GetVerifierAndApproverAsync(criteria);
    }

    public async Task<RequestEntity> GetCheckInOutDetailToReviewDiscipline(long requestId)
    {
        var criteria = new GetCheckInOutDetailToReviewDisciplineCriteria
        {
            RequestId = requestId
        };
        return await requestRepository.GetRequestToIgnoreById(criteria);
    }

    public async Task<RequestEntity> GetLateInEarlyOutDetailToReviewDiscipline(long requestId)
    {
        var criteria = new GetLateInEarlyOutDetailToReviewDisciplineCriteria
        {
            RequestId = requestId
        };
        return await requestRepository.GetRequestToIgnoreById(criteria);
    }

    public async Task<IList<ChangeShiftDetail>> GetChangeShiftDetailToReviewDiscipline(long requestId)
    {
        var criteria = new GetChangeShiftDetailToReviewDisciplineCriteria
        {
            RequestId = requestId
        };
        return (await baseService.GetDataAsync<ChangeShiftDetail>(criteria)).ToList();
    }

    public async Task<IList<LeaveDisciplineModel>> GetLeaveRequestDetailToReviewDiscipline(long requestId)
    {
        var criteria = new GetLeaveRequestDetailToReviewDisciplineCriteria
        {
            RequestId = requestId
        };
        var requests = await requestRepository.GetLeaveRequestToReviewDiscipline(criteria);
        foreach (var request in requests)
        {
            foreach (var detail in request.LeaveDetail)
            {
                detail.FromTime = detail.FromTime.ToClientTimeZone();
                detail.ToTime = detail.ToTime.ToClientTimeZone();
            }
        }
        return requests;
    }

    public async Task<CommandResult> ApproveRequest(int actorId, ProcessRequestParam param)
    {
        var criteria = new ApproveRequestCriteria
        {
            RequestId = param.RequestId,
            RoleId = param.RoleId,
            Comment = param.Comment,
            ActorId = actorId,
            DisciplineXml = param.Disciplines.Any() ? param.Disciplines.ToXML().ToString() : string.Empty,
            ViewType = param.ViewType
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> ApproveRequests(ApproveRequestsParam param)
    {
        var criteria = new ApproveMultipleRequestsCriteria
        {
            RoleId = param.RoleId,
            Comment = param.Comment,
            ActorId = param.ActorId,
            IsIgnoredViolate = param.IsIgnoredViolate,
            IsConfirmed = param.IsConfirmed,
            Requests = param.Requests.Any() ? param.Requests.ToXML().ToString() : string.Empty
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> RejectRequest(int actorId, ProcessRequestParam param)
    {
        var companyId = session.Get<short>(SessionKey.CurrentCompanyId);
        if (companyId == 0)
        {
            var company = await companyService.GetCompanySiteByDomainAsync();
            companyId = company.CompanyId;
        }
        var criteria = new RejectRequestCriteria
        {
            RequestId = param.RequestId,
            RequestTypeId = param.RequestTypeId,
            RoleId = param.RoleId,
            Comment = param.Comment,
            ActorId = actorId,
            CompanyId = companyId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> ResetRequest(int actorId, ProcessRequestParam param)
    {
        var companyId = session.Get<short>(SessionKey.CurrentCompanyId);
        if (companyId == 0)
        {
            var company = await companyService.GetCompanySiteByDomainAsync();
            companyId = company.CompanyId;
        }
        var criteria = new ResetRequestCriteria
        {
            RequestId = param.RequestId,
            RequestTypeId = param.RequestTypeId,
            RoleId = param.RoleId,
            ActorId = actorId,
            CompanyId = companyId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> VoidRequest(int actorId, ProcessRequestParam param)
    {
        var companyId = session.Get<short>(SessionKey.CurrentCompanyId);
        if (companyId == 0)
        {
            var company = await companyService.GetCompanySiteByDomainAsync();
            companyId = company.CompanyId;
        }
        var criteria = new VoidRequestCriteria
        {
            RequestId = param.RequestId,
            RequestTypeId = param.RequestTypeId,
            Comment = param.Comment,
            ActorId = actorId,
            CompanyId = companyId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> SendNexHrmProcessRequest(int actorId, ProcessRequestParam param)
    {
        var baseUrl = nexConfig.GetBaseUrlByCompanyId(param.RequesterCompanyId);
        var command = param.Command is RequestCommand.Reset ? "Disapprove" : param.Command.ToString();
        var url = $"Request/ProcessRequest?requestId={param.RequestId}&command={command}&processedBy={actorId}";
        return await SendAsync<CommandResult>(baseUrl, url, HttpMethod.Post);
    }

    private async Task<T> SendAsync<T>(Uri baseUrl, string pathUrl, HttpMethod method)
    {
        var token = await GetTokenAsync(baseUrl);
        var client = clientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var url = new Uri($"{baseUrl}{pathUrl}");
        using var request = new HttpRequestMessage(method, url);
        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var jsonResult = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(jsonResult);
    }

    private async Task<string> GetTokenAsync(Uri baseUrl)
    {
        var url = new Uri($"{baseUrl}Token/GetToken?username={nexConfig.NexHRId}&password={nexConfig.NexHRPassword}");

        var client = clientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        using var response = await client.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task SendMailProcessRequest(ProcessRequestParam param, EmployeeSession actor, RequestCommand command)
    {
        switch (param.RequestTypeId)
        {
            case RequestType.CheckIn:
            case RequestType.CheckOut:
                await checkInOutRequestService.SendMailProcessRequestAsync(param.RequestId, actor, command);
                break;

            case RequestType.LateIn:
            case RequestType.EarlyOut:
                await lateInEarlyOutRequestService.SendMailProcessRequestAsync(param.RequestId, actor, command);
                break;

            case RequestType.ChangeShift:
                await changeShiftRequestService.SendMailProcessRequestAsync(param.RequestId, actor, command);
                break;

            case RequestType.Leave:
                await leaveRequestService.SendMailProcessRequest(param.RequestId, actor, command);
                break;

            case RequestType.KeepInView:
                await keepInViewRequestService.SendMailProcessRequestAsync(param.RequestId, actor, command);
                break;

            default:
                // Handle other request types
                break;
        }
    }

    public async Task SendMailProcessRequests(ApproveRequestsParam param, EmployeeSession actor, RequestCommand command)
    {
        foreach (var request in param.Requests)
        {
            switch (request.RequestTypeID)
            {
                case (int)RequestType.CheckIn:
                case (int)RequestType.CheckOut:
                    await checkInOutRequestService.SendMailProcessRequestAsync(request.RequestId, actor, command);
                    break;

                case (int)RequestType.LateIn:
                case (int)RequestType.EarlyOut:
                    await lateInEarlyOutRequestService.SendMailProcessRequestAsync(request.RequestId, actor, command);
                    break;

                case (int)RequestType.ChangeShift:
                    await changeShiftRequestService.SendMailProcessRequestAsync(request.RequestId, actor, command);
                    break;

                case (int)RequestType.Leave:
                    await leaveRequestService.SendMailProcessRequest(request.RequestId, actor, command);
                    break;

                case (int)RequestType.KeepInView:
                    await keepInViewRequestService.SendMailProcessRequestAsync(request.RequestId, actor, command);
                    break;

                default:
                    // Handle other request types
                    break;
            }
        }
    }

    private static string GetMessageForLeave(RequestEntity request, bool isNoFormat)
    {
        var msgTemplate = isNoFormat ? RequestFormatConstants.MsgLeaveWithNoFormat : RequestFormatConstants.MsgLeave;
        var leaveDateTime = GetDateTimeForLeaveRequest(request);
        var leaveDayDetail = GetLeaveDayDetail(request);
        var messageDetail = string.Format(msgTemplate, leaveDateTime, leaveDayDetail);

        return messageDetail;
    }

    private static string GetDateTimeForLeaveRequest(RequestEntity request)
    {
        var leaveDate = Utilities.GetMessageDateString(request.FromDate, request.ToDate);

        if (request.TotalDays - Constants.HalfDayOffValue == 0)
        {
            leaveDate = string.Format(
                RequestFormatConstants.MsgLeaveHalfDayWithNoFormat,
                request.FromDate.ToDateMonthYearFormatedDate(),
                request.FromTime.ToShortTime() + ((request.FromTime - request.FromDate).Days > 0 ? "+" : string.Empty),
                request.ToTime.ToShortTime() + ((request.ToTime - request.FromDate).Days > 0 ? "+" : string.Empty));
        }

        return leaveDate;
    }

    private static string GetMessageForChangeShift(RequestEntity request, bool isNoFormat)
    {
        string msgTemplate;
        string messageDetail;

        if (request.ChangeShiftType == (byte)ChangeShiftType.MoveOffDay)
        {
            var moveOffDayBuilder = new StringBuilder();
            foreach (var changeOffDate in request.ChangeShiftDetails)
            {
                msgTemplate = RequestFormatConstants.MsgChangeShift;

                var dateFormatted = GetMessageForMoveOffDay(changeOffDate.OnDate, changeOffDate.ToDate ?? changeOffDate.OnDate);
                moveOffDayBuilder.Append(string.Format(msgTemplate, dateFormatted));
            }

            moveOffDayBuilder.Append("Move off day");
            messageDetail = moveOffDayBuilder.ToString();
        }
        else
        {
            if (request.FromDate.Date == request.ToDate.Date)
            {
                msgTemplate = isNoFormat ? RequestFormatConstants.MsgChangeShiftOneDayWithNoFormat : RequestFormatConstants.MsgChangeShiftOneDay;
                messageDetail = string.Format(
                    msgTemplate,
                    request.FromShift.Name,
                    request.ToShift.Name,
                    request.FromDate.ToDateMonthYearFormatedDate());
            }
            else
            {
                msgTemplate = isNoFormat ? RequestFormatConstants.MsgChangeShiftWithNoFormat : RequestFormatConstants.MsgChangeShift;
                var date = Utilities.GetMessageDateString(request.FromDate, request.ToDate);
                messageDetail = string.Format(msgTemplate, date);
            }
        }

        return messageDetail;
    }

    private static string GetMessageForMoveOffDay(DateTime fromDate, DateTime toDate)
    {
        const string arrowSymbol = " <span class='arr'>&rarr;</span> ";
        if (fromDate.Year != toDate.Year)
        {
            return fromDate.ToDateMonthYearFormatedDate() + arrowSymbol + toDate.ToDateMonthYearFormatedDate();
        }

        if (fromDate.Month != toDate.Month)
        {
            return fromDate.ToString("dd") + arrowSymbol + toDate.ToDateMonthYearFormatedDate();
        }

        return fromDate.ToString("dd") + arrowSymbol + toDate.ToDateMonthYearFormatedDate();
    }

    private static string GetMessageForCheckInOut(RequestEntity request, bool isNoFormat)
    {
        var nexCompanyIds = Constants.NexCompanyIds;
        var requestCompanyId = request.CompanyId;
        var isInNexCompany = nexCompanyIds != null && nexCompanyIds.Contains(requestCompanyId);
        var msgTemplate = isNoFormat ? RequestFormatConstants.MsgCheckInOutWithNoFormat : RequestFormatConstants.MsgCheckInOut;
        var time = request.RequestedDate.Day == request.FromDate.Day ? request.RequestedDate.ToShortTime() : request.RequestedDate.ToShortTime() + "+";
        if (isInNexCompany)
        {
            time = request.OriginalRequestedDate?.Day == request.FromDate.Day ? request.OriginalRequestedDate?.ToShortTime() : request.OriginalRequestedDate?.ToShortTime() + "+";
        }
        return string.Format(msgTemplate, time, request.FromShift?.Name, request.FromDate.ToDateMonthYearFormatedDate());
    }

    private static string GetMessageForLateIn(RequestEntity request, bool isNoFormat)
    {
        var msgTemplate = isNoFormat ? RequestFormatConstants.MsgLateInWithNoFormat : RequestFormatConstants.MsgLateIn;
        var actualLateIn = request.ActualLateInEarlyOut == 0 ? Labels.NA : "+" + request.ActualLateInEarlyOut + "'";

        return string.Format(msgTemplate, request.TotalMins, request.FromShift?.Name, Utilities.GetMessageDateString(request.FromDate, request.ToDate), actualLateIn);
    }

    private static string GetMessageForEarlyOut(RequestEntity request, bool isNoFormat)
    {
        var msgTemplate = isNoFormat ? RequestFormatConstants.MsgEarlyOutWithNoFormat : RequestFormatConstants.MsgEarlyOut;
        var actualEarlyOut = request.ActualLateInEarlyOut == 0 ? Labels.NA : "-" + request.ActualLateInEarlyOut + "'";

        return string.Format(msgTemplate, request.TotalMins, request.FromShift?.Name, Utilities.GetMessageDateString(request.FromDate, request.ToDate), actualEarlyOut);
    }

    private static string GetMessageForKeepInView(RequestEntity request, bool isNoFormat)
    {
        var messageDetail = request.KeepInViewType == (int)KeepInViewTypes.KeepInViewByDay
            ? GetMessageForKeepInViewByDay(request, isNoFormat)
            : GetMessageForKeepInViewByHour(request, isNoFormat);

        return messageDetail;
    }

    private static string GetMessageForKeepInViewByDay(RequestEntity request, bool isNoFormat)
    {
        string msgTemplate;
        string messageDetail;

        if (request.TotalDays.Equals(Constants.HalfDayOffValue))
        {
            msgTemplate = isNoFormat ? RequestFormatConstants.MsgKeepInViewHalfDayWithNoFormat : RequestFormatConstants.MsgKeepInViewHalfDay;
            messageDetail = string.Format(
                msgTemplate,
                request.FromDate.ToDateMonthYearFormatedDate(),
                request.FromTime.ToShortTime(),
                request.ToTime.ToShortTime());
        }
        else
        {
            msgTemplate = isNoFormat ? RequestFormatConstants.MsgKeepInViewOneDayWithNoFormat : RequestFormatConstants.MsgKeepInViewOneDay;
            messageDetail = string.Format(msgTemplate, request.FromDate.ToDateMonthYearFormatedDate());
        }

        return messageDetail;
    }

    private static string GetMessageForKeepInViewByHour(RequestEntity request, bool isNoFormat)
    {
        var msgTemplate = isNoFormat ? RequestFormatConstants.MsgKeepInViewByHoursWithNoFormat : RequestFormatConstants.MsgKeepInViewByHours;
        var minutes = (request.ToTime - request.FromTime).TotalMinutes;
        var hours = minutes / Constants.TotalMinuteInHour;
        return string.Format(
            msgTemplate,
            request.FromDate.ToDateMonthYearFormatedDate(),
            request.FromTime.ToShortTime(),
            request.ToTime.ToShortTime(),
            hours.ToString("0.#"));
    }

    private static string GetLeaveDayDetail(RequestEntity request)
    {
        var result = string.Empty;
        if (request.LeaveType != null)
        {
            switch (request.LeaveType.Id)
            {
                case (ushort)LeaveTypes.GeneralLeave:
                    result = request.LeaveCountDetails;
                    break;

                case (ushort)LeaveTypes.KeepInViewByHour:
                case (ushort)LeaveTypes.BusinessDuringWorking:
                    var minutes = (request.ToTime - request.FromTime).TotalMinutes;
                    result = $"{minutes / Constants.TotalMinuteInHour:0.#} hour(s)";
                    break;

                default:
                    result = GetTotalDay(request.TotalDays);
                    break;
            }
        }

        return result;
    }

    private static string GetTotalDay(decimal totalDays)
        => totalDays != 0 ? string.Format(RequestFormatConstants.MsgTotalLeaveDays, totalDays.ToString("0.#")) : string.Empty;

    public async Task<List<CommandResult>> SendNexHrmMultipleProcessRequest(int actorId, ApproveRequestsParam param)
    {
        var requestsByCompany = param.Requests.GroupBy(r => r.RequesterCompanyId);
        var results = new List<CommandResult>();

        foreach (var companyGroup in requestsByCompany)
        {
            var baseUrl = nexConfig.GetBaseUrlByCompanyId(companyGroup.Key);
            var requestIds = string.Join(",", companyGroup.Select(r => r.RequestId));
            var url = $"Request/ProcessMultipleRequest?requestIds={requestIds}&command={param.Command}&processedBy={actorId}";

            var result = await SendAsync<List<CommandResult>>(baseUrl, url, HttpMethod.Post);
            results.AddRange(result);
        }

        return results;
    }

    public async Task<IEnumerable<RequestEntity>> GetNexRequests(RequestViewModel model, EmployeeSession employeeSession)
    {
        List<RequestEntity> responseRequests = [];
        var status = requestRepository.GetNexRequestStatus(model.StatusIds);
        var requestType = requestRepository.GetNexRequestType(model.RequestType);
        int? leaveType = null;
        if (model.LeaveType != null)
        {
            leaveType = requestRepository.GetNexLeaveType(model.LeaveType.Value);
        }

        var supervisorCompanies = await employeeService.GetCompaniesBelongToSupervisor();

        if (!supervisorCompanies.Any())
        {
            return responseRequests;
        }
        const int AssistantLeader = 1;
        List<NexRequestModel> requests = [];
        var companies = await companyService.GetAllCompany();
        foreach (var companyId in supervisorCompanies)
        {
            var criteria = new GetNexRequestCriteria
            {
                EmpId = model.EmployeeId == -1 ? null : model.EmployeeId,
                FromDate = model.FromDate.ToStartOfDay(),
                ToDate = model.ToDate.ToEndOfDay(),
                Status = status,
                RequestType = requestType,
                IsFilterByEffectedDate = model.IsFilterByAffectedDate,
                ActorId = model.CurrentUserId,
                LeaveTypeId = leaveType,
                Role = AssistantLeader,
                ExcludedEmpID = model.CurrentUserId,
                CompanyId = companyId
            };

            var responseData = await baseService.GetDataAsync<NexRequestModel>(criteria);
            var mappedData = responseData.Select(item =>
            {
                item.CompanyId = companyId;
                item.Status = (byte)requestRepository.GetGroupByNexRequestStatus(item.Status);
                var companyName = companies
                    .Where(c => c.CompanyId == item.CompanyId)
                    .Select(c => c.Name)
                    .FirstOrDefault();
                item.Requester += $" ({companyName})";
                return item;
            });
            requests.AddRange(mappedData);
        }

        responseRequests = FilterByEmployeeStatus(requestRepository.MappingNexRequestToGroupRequest(requests, employeeSession), model.EmployeeStatusIds)
            .OrderByDescending(item => item.CreatedDate)
            .ToList();

        return responseRequests;
    }

    private static IEnumerable<RequestEntity> FilterByEmployeeStatus(
    IEnumerable<RequestEntity> requests,
    string employeeStatusIds)
    {
        var selectedStatuses = (employeeStatusIds ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(byte.Parse)
            .ToList();

        return requests.Where(i =>
            !selectedStatuses.Any() ||
            selectedStatuses.Exists(s => MatchStatus(i, s))
        );
    }

    private static bool MatchStatus(RequestEntity request, byte status)
    {
        const byte Enable = 1;
        const byte Disabled = 2;
        const byte Deleted = 3;

        return status switch
        {
            Enable => request.Requester.Enabled && !request.Requester.IsDeleted,
            Disabled => !request.Requester.Enabled && !request.Requester.IsDeleted,
            Deleted => request.Requester.IsDeleted,                               
            _ => false
        };
    }
}