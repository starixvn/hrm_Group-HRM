using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using HRM.Models.Report;
using HRM.Models.ScheduleArrange;

namespace HRM.Repositories.Interfaces;

public interface IWorkingCalendarRepository
{
    Task<IEnumerable<DailySchedule>> GetWorkingScheduleAsync(ICriteria criteria);

    Task<IEnumerable<WorkingCalendar>> GetWorkingCalendarsAsync(ICriteria criteria);
}