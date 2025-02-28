using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Criteria.Request.LateInEarlyOut;
using HRM.Models.Request;

namespace HRM.Services.Interfaces.Request;

public interface ILateInEarlyOutRequestService
{
    Task<IList<LateInEarlyOutRequest>> GetByDateAsync(GetLateInEarlyOutRequestByDateCriteria criteria);
    Task<LateInEarlyOutRequest> GetByIdAsync(long requestId);
    Task<IList<LateInEarlyOutRequest>> GetHistoryByIdAsync(long requestId);
    Task<IEnumerable<DateTime>> GetDisabledDateAsync(GetDisabledDateParam param);
    Task<CommandResult> DeleteAsync(long requestId);
    Task<EmployeeLateInEarlyOutModel> GetActualLateInEarlyOutAsync(GetActualLateInEarlyOutCriteria criteria);
    Task SendMailAndSendLetstalkCreateRequestAsync(long requestId);
    Task SendMailEditRequestAsync(long requestId);
    Task SendMailDeleteRequestAsync(long requestId);
    Task SendMailProcessRequestAsync(long requestId, EmployeeSession actor, RequestCommand command);
}