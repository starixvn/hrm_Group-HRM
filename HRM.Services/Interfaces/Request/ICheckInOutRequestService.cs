namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Criteria.Request.CheckInOut;
    using HRM.Models.Request;

    public interface ICheckInOutRequestService
    {
        IList<CheckInOutRequest> GetByDate(GetCheckInOutRequestByDateCriteria criteria);

        CheckInOutRequest GetById(long requestId);

        IList<CheckInOutRequest> GetHistoryById(long requestId);

        IEnumerable<DateTime> GetDisabledDate(GetDisabledDateParam param);

        CommandResult Delete(long requestId);

        void SendMailAndSendLetstalkCreateRequest(long requestId);

        void SendMailEditRequest(long requestId);

        void SendMailDeleteRequest(long requestId);

        void SendMailProcessRequest(long requestId, EmployeeSession actor, RequestCommand command);
    }
}