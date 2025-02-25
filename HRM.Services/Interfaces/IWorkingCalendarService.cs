namespace HRM.Services
{
    using System.Collections.Generic;
    using HRM.Criteria.Calendar;
    using HRM.Models.Report;
    using HRM.Models.ScheduleArrange;

    public interface IWorkingCalendarService
    {
        IEnumerable<DailySchedule> GetWorkingSchedule(GetEmployeeCalendarByMonthCriteria criteria);

        IEnumerable<WorkingCalendar> GetWorkingCalendars(GetWorkingCalendarWeeklyCriteria criteria);
    }
}