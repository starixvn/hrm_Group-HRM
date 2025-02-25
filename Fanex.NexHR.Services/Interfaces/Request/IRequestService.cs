namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Request.ChangeShift;

    public interface IRequestService
    {
        IList<RequestEntity> GetRequests(GetRequestCriteria criteria);

        VerifierAndApproverForRequest GetVerifierAndApprover(int employeeId, RequestType requestType);

        RequestEntity GetCheckInOutDetailToReviewDiscipline(long requestId);

        RequestEntity GetLateInEarlyOutDetailToReviewDiscipline(long requestId);

        IList<ChangeShiftDetail> GetChangeShiftDetailToReviewDiscipline(long requestId);

        IList<LeaveDisciplineModel> GetLeaveRequestDetailToReviewDiscipline(long requestId);

        CommandResult ApproveRequest(int actorId, ProcessRequestParam param);

        CommandResult ApproveRequests(ApproveRequestsParam param);

        CommandResult ResetRequest(int actorId, ProcessRequestParam param);

        CommandResult RejectRequest(int actorId, ProcessRequestParam param);

        CommandResult VoidRequest(int actorId, ProcessRequestParam param);

        void SendMailProcessRequest(ProcessRequestParam param, EmployeeSession actor, RequestCommand command);

        void SendMailProcessRequests(ApproveRequestsParam param, EmployeeSession actor, RequestCommand command);
    }
}