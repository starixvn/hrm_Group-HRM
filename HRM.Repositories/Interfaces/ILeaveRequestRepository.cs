namespace HRM.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRM.Models.Request;

    public interface ILeaveRequestRepository
    {
        Task<IList<LeaveRequest>> GetByDateAsync(ICriteria criteria);

        LeaveRequest GetById(ICriteria criteria);

        IEnumerable<LeaveRequest> GetHistoryById(ICriteria criteria);

        IEnumerable<Models.ScheduleArrange.DailySchedule> GetEmployeeCalendar(ICriteria criteria);
    }
}