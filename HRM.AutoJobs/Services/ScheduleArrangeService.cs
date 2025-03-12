using HRM.AutoJobs.Services.Interfaces;

namespace HRM.AutoJobs.Services;

using System;
using System.Threading.Tasks;
using Criteria;

public class ScheduleArrangeService(IBaseService baseService) : IScheduleArrangeService
{
    public async Task PublicScheduleAsync()
    {
        var command = new PublicScheduleCommand();
        await baseService.ExecuteNonQueryAsync(command);
    }

    public async Task InsertDefaultShiftAsync()
    {
        var fromDate = LastDayOfWeek(DateTimeOffset.Now.Date).AddDays(1);
        var toDate = fromDate.AddDays(6);
        var command = new InsertScheduleByDefaultShiftCommand
        {
            FromDate = fromDate,
            ToDate = toDate
        };
        await baseService.ExecuteNonQueryAsync(command);
    }

    public async Task InsertWeeklyDefaultShiftAsync()
    {
        var fromDate = LastDayOfWeek(DateTimeOffset.Now.Date).AddDays(1);
        var toDate = fromDate.AddDays(6);
        var command = new InsertScheduleByWeeklyDefaultShiftCommand
        {
            FromDate = fromDate,
            ToDate = toDate
        };
        await baseService.ExecuteNonQueryAsync(command);
    }

    private static DateTime FirstDayOfWeek(DateTime dt)
    {
        var diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }

    private static DateTime LastDayOfWeek(DateTime dt)
    {
        const byte AddDays = 6;
        return FirstDayOfWeek(dt).AddDays(AddDays);
    }
}