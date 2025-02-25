namespace HRM.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRM.Models.Report;
    using HRM.Models.ScheduleArrange;

    public partial interface IReportRepository
    {
        IEnumerable<WorkingCalendarSummary> GetWorkingCalendarSummary(ICriteria criteria);
    }

    public partial class ReportRepository : IReportRepository
    {
        public IEnumerable<WorkingCalendarSummary> GetWorkingCalendarSummary(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<WorkingCalendarSummary, ShiftBase, LeaveRequest, LeaveRequest, WorkingCalendarSummary>(
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
    }
}