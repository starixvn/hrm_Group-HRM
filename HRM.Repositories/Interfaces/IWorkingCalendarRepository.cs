namespace HRM.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Models.Report;
using Models.ScheduleArrange;

public interface IWorkingCalendarRepository
{
    Task<IEnumerable<DailySchedule>> GetWorkingScheduleAsync(ICriteria criteria);

    Task<IEnumerable<WorkingCalendar>> GetWorkingCalendarsAsync(ICriteria criteria);
}