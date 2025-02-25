namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Criteria.Request.KeepInView;
    using HRM.Models.Request;

    public interface IKeepInViewRequestService
    {
        IList<KeepInViewRequest> GetByDate(GetKeepInViewRequestByDateCriteria criteria);

        KeepInViewRequest GetById(long requestId);

        IList<KeepInViewRequest> GetHistoryById(long requestId);

        IEnumerable<DateTime> GetDisabledDate(GetDisabledDateParam param);

        CommandResult Delete(long requestId);

        void SendMailAndSendLetstalkCreateRequest(long requestId);

        void SendMailEditRequest(long requestId);

        void SendMailDeleteRequest(long requestId);

        void SendMailProcessRequest(long requestId, EmployeeSession actor, RequestCommand command);
    }
}