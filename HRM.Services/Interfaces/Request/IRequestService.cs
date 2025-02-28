namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Criteria.Request;
    using HRM.Models.Request;
    using HRM.Models.Request.ChangeShift;

    public interface IRequestService
    {
        IList<RequestEntity> GetRequests(GetRequestCriteria criteria);

        Task<VerifierAndApproverForRequest> GetVerifierAndApproverAsync(int employeeId, RequestType requestType);

        RequestEntity GetCheckInOutDetailToReviewDiscipline(long requestId);

        RequestEntity GetLateInEarlyOutDetailToReviewDiscipline(long requestId);

        IList<ChangeShiftDetail> GetChangeShiftDetailToReviewDiscipline(long requestId);

        IList<LeaveDisciplineModel> GetLeaveRequestDetailToReviewDiscipline(long requestId);

        CommandResult ApproveRequest(int actorId, ProcessRequestParam param);

        CommandResult ApproveRequests(ApproveRequestsParam param);

        CommandResult ResetRequest(int actorId, ProcessRequestParam param);

        CommandResult RejectRequest(int actorId, ProcessRequestParam param);

        Task<CommandResult> VoidRequestAsync(int actorId, ProcessRequestParam param);

        CommandResult VoidRequest(int actorId, ProcessRequestParam param);

        void SendMailProcessRequest(ProcessRequestParam param, EmployeeSession actor, RequestCommand command);

        void SendMailProcessRequests(ApproveRequestsParam param, EmployeeSession actor, RequestCommand command);
    }
}