namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Request.Leave;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public interface ILeaveRequestService
    {
        IList<LeaveRequest> GetByDate(GetLeaveRequestByDateCriteria criteria);

        LeaveRequest GetById(long requestId);

        IList<LeaveRequestDetail> GetLeaveRequestDetailById(long requestId);

        IEnumerable<LeaveRequest> GetHistoryById(long requestId);

        IEnumerable<DateTime> GetDisabledDate(GetDisabledDateParam param);

        IEnumerable<Models.ScheduleArrange.DailySchedule> GetEmployeeCalendar(int employeeId, DateTime fromDate, DateTime toDate);

        EmployeeLeaveBalance GetLeaveBalance(GetLeaveBalanceCriteria criteria);

        DateTime GetFromTimeHalfDay(IList<LeaveRequestDetail> leaveDetails);

        DateTime GetToTimeHalfDay(IList<LeaveRequestDetail> leaveDetails);

        CommandResult Delete(long requestId);

        void SendMailAndSendLetstalkCreateRequest(InsertLeaveRequestCriteria criteria);

        void SendMailEditRequest(UpdateLeaveRequestCriteria criteria);

        void SendMailDeleteRequest(long requestId);

        void SendMailProcessRequest(long requestId, EmployeeSession actor, RequestCommand command);
    }
}