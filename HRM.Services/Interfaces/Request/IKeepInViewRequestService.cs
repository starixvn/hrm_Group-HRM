namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Enums;
    using HRM.Core.Models;
    using Criteria.Request.KeepInView;
    using Models.Request;

    public interface IKeepInViewRequestService
    {
        Task<IList<KeepInViewRequest>> GetByDateAsync(GetKeepInViewRequestByDateCriteria criteria);
        Task<KeepInViewRequest> GetByIdAsync(long requestId);
        Task<IList<KeepInViewRequest>> GetHistoryByIdAsync(long requestId);
        Task<IEnumerable<DateTime>> GetDisabledDateAsync(GetDisabledDateParam param);
        Task<CommandResult> DeleteAsync(long requestId);
        Task SendMailAndSendLetstalkCreateRequestAsync(long requestId);
        Task SendMailEditRequestAsync(long requestId);
        Task SendMailDeleteRequestAsync(long requestId);
        Task SendMailProcessRequestAsync(long requestId, EmployeeSession actor, RequestCommand command);
    }
}