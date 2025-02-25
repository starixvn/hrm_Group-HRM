namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Request.KeepInView;
    using Fanex.NexHR.Models.Request;

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