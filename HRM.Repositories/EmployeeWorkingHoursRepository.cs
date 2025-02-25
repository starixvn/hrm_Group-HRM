namespace HRM.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRM.Models;
    using HRM.Models.Report.MonthlyWorkingHours;

    public class EmployeeWorkingHoursRepository : IEmployeeWorkingHoursRepository
    {
        public IEnumerable<MonthlyWorking> GetMonthlyWorkingHours(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    IEnumerable<MonthlyWorking> monthlyWorkings = multi.Read<MonthlyWorking, EntityBase<int>, MonthlyWorking>(
                        (monthlyWorking, team) =>
                        {
                            monthlyWorking.Team = team;
                            return monthlyWorking;
                        },
                        splitOn: "Id");

                    IEnumerable<DailyWorking> dailyWorkings = multi.Read<DailyWorking, EntityBase<int>, EntityBase<int>, DailyWorking>(
                        (dailyWorking, leave, leave2) =>
                        {
                            dailyWorking.LeaveType = leave;
                            dailyWorking.LeaveType2 = leave2;
                            return dailyWorking;
                        },
                        splitOn: "LeaveTypeId, LeaveTypeId2");

                    MappingData(monthlyWorkings, dailyWorkings);

                    return monthlyWorkings;
                }
            }
        }

        private static void MappingData(
            IEnumerable<MonthlyWorking> monthlyWorkings,
            IEnumerable<DailyWorking> dailyWorkings)
        {
            foreach (MonthlyWorking monthlyWorking in monthlyWorkings)
            {
                monthlyWorking.DailyWorkings = dailyWorkings.Where(x => x.EmployeeId == monthlyWorking.EmployeeId).OrderBy(x => x.WorkDate);
            }
        }
    }
}