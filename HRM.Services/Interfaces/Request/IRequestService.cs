using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Criteria.Request;
using HRM.Models.Request;
using HRM.Models.Request.ChangeShift;

namespace HRM.Services.Interfaces.Request;

public interface IRequestService
{
    Task<IList<RequestEntity>> GetRequestAsync(GetRequestCriteria criteria);

    Task<VerifierAndApproverForRequest> GetVerifierAndApproverAsync(int employeeId, RequestType requestType);

    Task<RequestEntity> GetCheckInOutDetailToReviewDiscipline(long requestId);

    Task<RequestEntity> GetLateInEarlyOutDetailToReviewDiscipline(long requestId);

    Task<IList<ChangeShiftDetail>> GetChangeShiftDetailToReviewDiscipline(long requestId);

    Task<IList<LeaveDisciplineModel>> GetLeaveRequestDetailToReviewDiscipline(long requestId);

    Task<CommandResult> ApproveRequest(int actorId, ProcessRequestParam param);

    Task<CommandResult> ApproveRequests(ApproveRequestsParam param);

    Task<CommandResult> ResetRequest(int actorId, ProcessRequestParam param);

    Task<CommandResult> RejectRequest(int actorId, ProcessRequestParam param);

    Task<CommandResult> VoidRequest(int actorId, ProcessRequestParam param);

    Task SendMailProcessRequest(ProcessRequestParam param, EmployeeSession actor, RequestCommand command);

    Task SendMailProcessRequests(ApproveRequestsParam param, EmployeeSession actor, RequestCommand command);

    Task<CommandResult> SendNexHrmProcessRequest(int actorId, ProcessRequestParam param);

    Task<List<CommandResult>> SendNexHrmMultipleProcessRequest(int actorId, ApproveRequestsParam param);
}