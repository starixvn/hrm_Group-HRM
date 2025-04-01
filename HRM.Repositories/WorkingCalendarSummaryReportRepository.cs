namespace HRM.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data;
using Fanex.Data.Repository;
using Models.Report;
using Models.ScheduleArrange;

public interface IWorkingCalendarSummaryReportRepository
{
    Task<IEnumerable<WorkingCalendarSummary>> GetWorkingCalendarSummaryAsync(ICriteria criteria);
}

public class WorkingCalendarSummaryReportRepository : IWorkingCalendarSummaryReportRepository
{
    public async Task<IEnumerable<WorkingCalendarSummary>> GetWorkingCalendarSummaryAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return await db.QueryAsync<WorkingCalendarSummary, ShiftBase, LeaveRequest, LeaveRequest, WorkingCalendarSummary>(
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
}