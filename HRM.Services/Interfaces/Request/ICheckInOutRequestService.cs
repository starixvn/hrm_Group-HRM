using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Criteria.Request.CheckInOut;
using HRM.Models;
using HRM.Models.Request;

namespace HRM.Services.Interfaces.Request;

public interface ICheckInOutRequestService
{
    Task<IList<CheckInOutRequest>> GetByDateAsync(GetCheckInOutRequestByDateCriteria criteria);
    Task<CheckInOutRequest> GetByIdAsync(long requestId);
    Task<IList<CheckInOutRequest>> GetHistoryByIdAsync(long requestId);
    Task<IEnumerable<DateTime>> GetDisabledDateAsync(GetDisabledDateParam param);
    Task<CommandResult> DeleteAsync(long requestId);
    Task SendMailAndSendLetstalkCreateRequestAsync(long requestId);
    Task SendMailEditRequestAsync(long requestId);
    Task SendMailDeleteRequestAsync(long requestId);
    Task SendMailProcessRequestAsync(long requestId, EmployeeSession actor, RequestCommand command);
}