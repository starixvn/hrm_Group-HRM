namespace HRM.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using Fanex.Data.Repository;
using Interfaces;
using Models;
using Models.ScheduleArrange;

public class ScheduleArrangeRepository : IScheduleArrangeRepository
{
    public async Task<List<WeeklySchedule>> GetWeeklyScheduleAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var weeklySchedules = multi.Read<WeeklySchedule, EntityBase<int>, ShiftBase, WeeklySchedule>(
            (weeklySchedule, team, defaultShift) =>
            {
                weeklySchedule.Team = team;
                weeklySchedule.DefaultShift = defaultShift;

                return weeklySchedule;
            },
            splitOn: "TeamId, DefaultShiftId").ToList();

        var dailySchedules = multi.Read<DailySchedule, ShiftBase, LeaveRequest, LeaveRequest, DailySchedule>(
            (dailySchedule, shift, leave, leave2) =>
            {
                dailySchedule.Shift = shift;
                dailySchedule.LeaveType = leave;
                dailySchedule.LeaveType2 = leave2;

                return dailySchedule;
            },
            splitOn: "ShiftId, LeaveTypeId, LeaveTypeId2");

        var weeklyDefaultShifts = multi.Read<WeeklyDefaultShift>();

        MappingData(weeklySchedules, dailySchedules.ToList(), weeklyDefaultShifts.ToList());

        return weeklySchedules;
    }

    private static void MappingData(
        IEnumerable<WeeklySchedule> weeklySchedules,
        List<DailySchedule> dailySchedules,
        List<WeeklyDefaultShift> weeklyDefaultShifts)
    {
        foreach (var weeklySchedule in weeklySchedules)
        {
            weeklySchedule.WeeklyDefaultShift = weeklyDefaultShifts.Find(x => x.EmployeeId == weeklySchedule.EmployeeId);
            var memberDailySchedules = dailySchedules.Where(x => x.EmployeeId == weeklySchedule.EmployeeId);

            foreach (var dailySchedule in memberDailySchedules)
            {
                switch (dailySchedule.WorkDate.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        weeklySchedule.MondaySchedule = dailySchedule;
                        break;

                    case DayOfWeek.Tuesday:
                        weeklySchedule.TuesdaySchedule = dailySchedule;
                        break;

                    case DayOfWeek.Wednesday:
                        weeklySchedule.WednesdaySchedule = dailySchedule;
                        break;

                    case DayOfWeek.Thursday:
                        weeklySchedule.ThursdaySchedule = dailySchedule;
                        break;

                    case DayOfWeek.Friday:
                        weeklySchedule.FridaySchedule = dailySchedule;
                        break;

                    case DayOfWeek.Saturday:
                        weeklySchedule.SaturdaySchedule = dailySchedule;
                        break;

                    case DayOfWeek.Sunday:
                        weeklySchedule.SundaySchedule = dailySchedule;
                        break;

                    default:
                        // Handle other days of the week
                        break;
                }
            }
        }
    }
}