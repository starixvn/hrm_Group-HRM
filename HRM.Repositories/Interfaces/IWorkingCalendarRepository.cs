namespace HRM.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using HRM.Models.Report;
    using HRM.Models.ScheduleArrange;

    public interface IWorkingCalendarRepository
    {
        IEnumerable<DailySchedule> GetWorkingSchedule(ICriteria criteria);

        IEnumerable<WorkingCalendar> GetWorkingCalendars(ICriteria criteria);
    }
}