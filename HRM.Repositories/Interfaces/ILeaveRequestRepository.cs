namespace HRM.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Models.Request;

public interface ILeaveRequestRepository
{
    Task<IList<LeaveRequest>> GetByDateAsync(ICriteria criteria);

    Task<LeaveRequest> GetByIdAsync(ICriteria criteria);

    Task<IEnumerable<LeaveRequest>> GetHistoryByIdAsync(ICriteria criteria);

    Task<IEnumerable<Models.ScheduleArrange.DailySchedule>> GetEmployeeCalendarAsync(ICriteria criteria);
}