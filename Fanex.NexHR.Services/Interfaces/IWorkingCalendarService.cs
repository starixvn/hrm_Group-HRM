namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.ScheduleArrange;

    public interface IWorkingCalendarService
    {
        IEnumerable<DailySchedule> GetWorkingSchedule(GetEmployeeCalendarByMonthCriteria criteria);

        IEnumerable<WorkingCalendar> GetWorkingCalendars(GetWorkingCalendarWeeklyCriteria criteria);
    }
}