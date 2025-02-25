namespace HRM.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using HRM.Models.Request;

    public interface ILeaveRequestRepository
    {
        IList<LeaveRequest> GetByDate(ICriteria criteria);

        LeaveRequest GetById(ICriteria criteria);

        IEnumerable<LeaveRequest> GetHistoryById(ICriteria criteria);

        IEnumerable<Models.ScheduleArrange.DailySchedule> GetEmployeeCalendar(ICriteria criteria);
    }
}