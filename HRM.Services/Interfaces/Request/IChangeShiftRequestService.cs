using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Criteria.Request.ChangeShift;
using HRM.Models;
using HRM.Models.Request.ChangeShift;
using HRM.Models.ScheduleArrange;

namespace HRM.Services.Interfaces.Request;

public interface IChangeShiftRequestService
{
    Task<int> GetNumberOfRequestAsync(int employeeId, DateTime fromDate, DateTime toDate);

    Task<IEnumerable<DailySchedule>> GetWorkingCalendarForRequestAsync(GetWorkingCalendarForRequestParam param);

    Task<IEnumerable<DateTime>> GetDisableDateForMoveOffDayRequestAsync(int employeeId, DateTime requestDate, long? requestId = null);

    Task<IEnumerable<ChangeShiftRequest>> GetByEmployeeAsync(GetChangeShiftRequestByEmployeeCriteria criteria);

    Task<IEnumerable<ChangeShiftRequest>> GetHistoryByIdAsync(long requestId);

    Task<ChangeShiftRequest> GetByIdAsync(long requestId);

    Task<IEnumerable<ChangeShiftDetail>> GetDetailByIdAsync(long requestId);

    Task<CommandResult> CreateAsync(int actorId, CreateChangeShift model);

    Task<CommandResult> EditAsync(int actorId, EditChangeShift model);

    Task<CommandResult> DeleteAsync(int actorId, long requestId);

    Task SendMailAndSendLetstalkCreateRequestAsync(long requestId, MailConfigModel mailConfigModel);

    Task SendMailEditRequestAsync(long requestId, MailConfigModel mailConfigModel);

    Task SendMailDeleteRequestAsync(long requestId);

    Task SendMailProcessRequestAsync(long requestId, EmployeeSession actor, RequestCommand command);
}