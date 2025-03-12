using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Criteria.Calendar;
using HRM.Models.Report;
using HRM.Models.ScheduleArrange;

namespace HRM.Services.Interfaces;

public interface IWorkingCalendarService
{
    Task<IEnumerable<DailySchedule>> GetWorkingScheduleAsync(GetEmployeeCalendarByMonthCriteria criteria);

    Task<IEnumerable<WorkingCalendar>> GetWorkingCalendarsAsync(GetWorkingCalendarWeeklyCriteria criteria);
}