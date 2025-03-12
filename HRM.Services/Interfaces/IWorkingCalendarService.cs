namespace HRM.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Criteria.Calendar;
using Models.Report;
using Models.ScheduleArrange;

public interface IWorkingCalendarService
{
    Task<IEnumerable<DailySchedule>> GetWorkingScheduleAsync(GetEmployeeCalendarByMonthCriteria criteria);

    Task<IEnumerable<WorkingCalendar>> GetWorkingCalendarsAsync(GetWorkingCalendarWeeklyCriteria criteria);
}