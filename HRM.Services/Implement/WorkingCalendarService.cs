namespace HRM.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Criteria.Calendar;
using Models.Report;
using Models.ScheduleArrange;
using Repositories;

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