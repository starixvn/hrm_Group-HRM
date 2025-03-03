namespace HRM.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Criteria.Calendar;
using HRM.Models.Report;
using HRM.Models.ScheduleArrange;
using HRM.Repositories;

public class WorkingCalendarService(IWorkingCalendarRepository workingCalendarRepository) : IWorkingCalendarService
{
    public async Task<IEnumerable<DailySchedule>> GetWorkingScheduleAsync(GetEmployeeCalendarByMonthCriteria criteria)
    {
        return await workingCalendarRepository.GetWorkingScheduleAsync(criteria);
    }

    public async Task<IEnumerable<WorkingCalendar>> GetWorkingCalendarsAsync(GetWorkingCalendarWeeklyCriteria criteria)
    {
        return await workingCalendarRepository.GetWorkingCalendarsAsync(criteria);
    }
}