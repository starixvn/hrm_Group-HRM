namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Criteria.Request.LateInEarlyOut;
    using HRM.Models.Request;

    public interface ILateInEarlyOutRequestService
    {
        IList<LateInEarlyOutRequest> GetByDate(GetLateInEarlyOutRequestByDateCriteria criteria);

        LateInEarlyOutRequest GetById(long requestId);

        IList<LateInEarlyOutRequest> GetHistoryById(long requestId);

        IEnumerable<DateTime> GetDisabledDate(GetDisabledDateParam param);

        CommandResult Delete(long requestId);

        EmployeeLateInEarlyOutModel GetActualLateInEarlyOut(GetActualLateInEarlyOutCriteria criteria);

        void SendMailAndSendLetstalkCreateRequest(long requestId);

        void SendMailEditRequest(long requestId);

        void SendMailDeleteRequest(long requestId);

        void SendMailProcessRequest(long requestId, EmployeeSession actor, RequestCommand command);
    }
}