namespace HRM.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRM.Models.Report;
    using HRM.Models.ScheduleArrange;

    public interface IWorkingCalendarRepository
    {
        Task<IEnumerable<DailySchedule>> GetWorkingScheduleAsync(ICriteria criteria);

        Task<IEnumerable<WorkingCalendar>> GetWorkingCalendarsAsync(ICriteria criteria);
    }
}