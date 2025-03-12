using HRM.Repositories.Interfaces;

namespace HRM.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using Fanex.Data.Repository;
using Models.Report;
using Models.ScheduleArrange;

public class WorkingCalendarRepository : IWorkingCalendarRepository
{
    public async Task<IEnumerable<DailySchedule>> GetWorkingScheduleAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return await db.QueryAsync<DailySchedule, ShiftBase, LeaveRequest, LeaveRequest, DailySchedule>(
            (dailySchedule, shift, leave, leave2) =>
            {
                dailySchedule.Shift = shift;
                dailySchedule.LeaveType = leave;
                dailySchedule.LeaveType2 = leave2;

                return dailySchedule;
            },
            splitOn: "ShiftId, LeaveTypeId, LeaveTypeId2",
            parameterSet: criteria);
    }

    public async Task<IEnumerable<WorkingCalendar>> GetWorkingCalendarsAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var calendars = await multi.ReadAsync<WorkingCalendar>();
        var dailySchedules = multi.Read<DailySchedule, ShiftBase, LeaveRequest, LeaveRequest, DailySchedule>(
            (dailySchedule, shift, leave, leave2) =>
            {
                dailySchedule.Shift = shift;
                dailySchedule.LeaveType = leave;
                dailySchedule.LeaveType2 = leave2;

                return dailySchedule;
            },
            splitOn: "ShiftId, LeaveTypeId, LeaveTypeId2");

        MappingData(calendars, dailySchedules);

        return calendars;
    }

    private static void MappingData(
        IEnumerable<WorkingCalendar> calendars,
        IEnumerable<DailySchedule> dailySchedules)
    {
        foreach (var calendar in calendars)
        {
            calendar.Schedules = dailySchedules.Where(x => x.EmployeeId == calendar.EmployeeId)
                .OrderBy(x => x.WorkDate);
        }
    }
}