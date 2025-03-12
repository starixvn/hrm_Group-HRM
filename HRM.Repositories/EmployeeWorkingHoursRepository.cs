namespace HRM.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using Fanex.Data.Repository;
using Models;
using Models.Report.MonthlyWorkingHours;

public class EmployeeWorkingHoursRepository : IEmployeeWorkingHoursRepository
{
    public async Task<IEnumerable<MonthlyWorking>> GetMonthlyWorkingHoursAsync(ICriteria criteria)
    {
        using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
        {
            using (var multi = await db.QueryMultipleAsync(criteria))
            {
                var monthlyWorkings = multi.Read<MonthlyWorking, EntityBase<int>, MonthlyWorking>(
                    (monthlyWorking, team) =>
                    {
                        monthlyWorking.Team = team;
                        return monthlyWorking;
                    },
                    splitOn: "Id");

                var dailyWorkings = multi.Read<DailyWorking, EntityBase<int>, EntityBase<int>, DailyWorking>(
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
        foreach (var monthlyWorking in monthlyWorkings)
        {
            monthlyWorking.DailyWorkings = dailyWorkings.Where(x => x.EmployeeId == monthlyWorking.EmployeeId).OrderBy(x => x.WorkDate);
        }
    }
}