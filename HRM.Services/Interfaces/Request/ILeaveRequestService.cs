namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Criteria.Request;
    using HRM.Criteria.Request.Leave;
    using HRM.Models;
    using HRM.Models.Request;

    public interface ILeaveRequestService
    {
        Task<IList<LeaveRequest>> GetByDateAsync(GetLeaveRequestByDateCriteria criteria);

        Task<LeaveRequest> GetByIdAsync(long requestId);

        Task<IList<LeaveRequestDetail>> GetLeaveRequestDetailByIdAsync(long requestId);

        Task<IEnumerable<LeaveRequest>> GetHistoryByIdAsync(long requestId);

        Task<IEnumerable<DateTime>> GetDisabledDateAsync(GetDisabledDateParam param);

        Task<IEnumerable<Models.ScheduleArrange.DailySchedule>> GetEmployeeCalendarAsync(int employeeId, DateTime fromDate, DateTime toDate);

        Task<EmployeeLeaveBalance> GetLeaveBalanceAsync(GetLeaveBalanceCriteria criteria);

        DateTime GetFromTimeHalfDay(IList<LeaveRequestDetail> leaveDetails);

        DateTime GetToTimeHalfDay(IList<LeaveRequestDetail> leaveDetails);

        Task<CommandResult> DeleteAsync(long requestId);

        void SendMailAndSendLetstalkCreateRequest(InsertLeaveRequestCriteria criteria);

        void SendMailEditRequest(UpdateLeaveRequestCriteria criteria);

        Task SendMailDeleteRequest(long requestId);

        Task SendMailProcessRequest(long requestId, EmployeeSession actor, RequestCommand command);
    }
}