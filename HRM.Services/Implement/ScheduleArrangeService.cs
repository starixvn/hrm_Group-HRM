namespace HRM.Services.Implement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.XMLExtensions;
using Criteria.ScheduleArrange;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Repositories.Interfaces;
using Interfaces;
using Models;
using Models.ScheduleArrange;
using Models.Shifts;

public class ScheduleArrangeService(
    IScheduleArrangeRepository scheduleArrangeRepository,
    IBaseService baseService,
    IShiftService shiftService,
    IUserControlSession userControlSession) : IScheduleArrangeService
{
    public async Task<IEnumerable<EmployeeSuggestion>> GetEmployeesAsync(GetEmployeeFilter filter)
    {
        var criteria = new GetEmployeeCriteria
        {
            ActorId = filter.ActorId,
            TeamIds = filter.TeamIds,
            Query = filter.Query,
            IsIFrame = filter.IsIFrame,
            IsEditScheduleUnlimited = filter.IsEditScheduleUnlimited
        };

        return await baseService.GetDataAsync<EmployeeSuggestion>(criteria);
    }

    public async Task<IEnumerable<WeeklySchedule>> GetWeeklyScheduleAsync(WeeklyScheduleFilter filter)
    {
        var currentCompanyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var companyId = filter.CompanyId ?? currentCompanyId;
        var criteria = new GetWeeklyScheduleCriteria
        {
            ActorId = filter.ActorId,
            FromTime = filter.FromDate,
            ToTime = filter.ToDate,
            EmpId = filter.EmployeeId,
            TeamIds = filter.TeamIds,
            IsIFrame = filter.IsIFrame,
            IsLocal = filter.IsLocal.HasValue ? filter.IsLocal.Value == 1 : null,
            CompanyId = companyId,
            CurrentCompanyId = currentCompanyId,
            IsEditScheduleUnlimited = filter.IsEditScheduleUnlimited
        };

        var weeklySchedules = await scheduleArrangeRepository.GetWeeklyScheduleAsync(criteria);
        var shifts = await shiftService.GetAllAsync(isShowOff: true, empCompanyId: companyId);
        MappingWeeklyDefaultShift(weeklySchedules, shifts);
        MappingDefaultShift(weeklySchedules);

        return weeklySchedules;
    }

    public async Task<CommandResult> EditWeeklyAsync(EditWeeklyScheduleCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> EditRemarkAsync(int actorId, EditRemark model)
    {
        var criteria = new EditRemarkCriteria
        {
            ActorId = actorId,
            EmployeeId = model.EmployeeId,
            Remark = model.Remark
        };

        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> EditDisplayOrderAsync(int actorId, IEnumerable<EditDisplayOrder> displayOrders)
    {
        var criteria = new EditDisplayOrderCriteria
        {
            ActorId = actorId,
            DisplayOrderXml = displayOrders.ToXML().ToString()
        };

        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> EditTeamOrderAsync(int actorId, IEnumerable<EditTeamOrder> displayOrders)
    {
        var criteria = new EditTeamOrderCriteria
        {
            ActorId = actorId,
            DisplayOrderXml = displayOrders.ToXML().ToString()
        };

        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> EditWeeklyDefaultShiftAsync(int actorId, WeeklyDefaultShift weeklyDefaultShift, bool isUsedWeeklyDefaultShift)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new EditWeeklyDefaultShiftCriteria
        {
            CompanyId = companyId,
            ActorId = actorId,
            EmployeeId = weeklyDefaultShift.EmployeeId,
            FromDate = DateTimeOffset.Now.Date.LastDayOfWeek().AddDays(1),
            MonShiftId = GetShiftId(weeklyDefaultShift.MonShift.Id),
            TueShiftId = GetShiftId(weeklyDefaultShift.TueShift.Id),
            WedShiftId = GetShiftId(weeklyDefaultShift.WedShift.Id),
            ThuShiftId = GetShiftId(weeklyDefaultShift.ThuShift.Id),
            FriShiftId = GetShiftId(weeklyDefaultShift.FriShift.Id),
            SatShiftId = GetShiftId(weeklyDefaultShift.SatShift.Id),
            SunShiftId = GetShiftId(weeklyDefaultShift.SunShift.Id),
            IsUsedWeeklyDefaultShift = isUsedWeeklyDefaultShift
        };

        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    private static int? GetShiftId(int? shiftId)
    {
        return shiftId.HasValue && shiftId.Value != Constants.OffDayShiftId ? shiftId.Value : null;
    }

    private static ShiftBase FindShiftOrDefault(IList<ShiftBase> shiftBases, int? shiftId)
    {
        if (!shiftId.HasValue)
        {
            return null;
        }

        return shiftBases.FirstOrDefault(x => x.Id == shiftId.Value) ??
               new ShiftBase { Id = shiftId.Value, Code = "Unknown", Name = "Unknown Shift" };
    }

    private static void MappingWeeklyDefaultShift(IEnumerable<WeeklySchedule> weeklySchedules, IList<Shift> shifts)
    {
        var shiftBases = shifts.Select(x => new ShiftBase
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name,
            CheckIn = x.BeginCheckInTime,
            CheckOut = x.BeginCheckOutTime,
            IsInAfterDay = x.IsInAfterDay,
            IsOutAfterDay = x.IsOutAfterDay
        }).ToList();

        var weeklyDefaultShifts = weeklySchedules.Select(x => x.WeeklyDefaultShift).ToList();
        foreach (var weeklyDefaultShift in weeklyDefaultShifts.Where(weeklyDefaultShift => weeklyDefaultShift != null))
        {
            weeklyDefaultShift.MonShift = shiftBases.First(x => x.Id == weeklyDefaultShift.MonShiftId);
            weeklyDefaultShift.TueShift = shiftBases.First(x => x.Id == weeklyDefaultShift.TueShiftId);
            weeklyDefaultShift.WedShift = shiftBases.First(x => x.Id == weeklyDefaultShift.WedShiftId);
            weeklyDefaultShift.ThuShift = shiftBases.First(x => x.Id == weeklyDefaultShift.ThuShiftId);
            weeklyDefaultShift.FriShift = shiftBases.First(x => x.Id == weeklyDefaultShift.FriShiftId);
            weeklyDefaultShift.SatShift = shiftBases.First(x => x.Id == weeklyDefaultShift.SatShiftId);
            weeklyDefaultShift.SunShift = shiftBases.First(x => x.Id == weeklyDefaultShift.SunShiftId);
        }
    }

    private static void MappingDefaultShift(IEnumerable<WeeklySchedule> weeklySchedules)
    {
        foreach (var weeklySchedule in weeklySchedules)
        {
            var weeklyDefaultShift = weeklySchedule.WeeklyDefaultShift;
            var defaultShift = weeklySchedule.DefaultShift;

            MappingDailyDefaultShift(
                weeklySchedule.MondaySchedule,
                weeklySchedule.IsUsedWeeklyDefaultShift
                    ? GetDefaultShift(weeklyDefaultShift, weeklySchedule.MondaySchedule.WorkDate.DayOfWeek) : defaultShift);
            MappingDailyDefaultShift(
                weeklySchedule.TuesdaySchedule,
                weeklySchedule.IsUsedWeeklyDefaultShift
                    ? GetDefaultShift(weeklyDefaultShift, weeklySchedule.TuesdaySchedule.WorkDate.DayOfWeek) : defaultShift);
            MappingDailyDefaultShift(
                weeklySchedule.WednesdaySchedule,
                weeklySchedule.IsUsedWeeklyDefaultShift
                    ? GetDefaultShift(weeklyDefaultShift, weeklySchedule.WednesdaySchedule.WorkDate.DayOfWeek) : defaultShift);
            MappingDailyDefaultShift(
                weeklySchedule.ThursdaySchedule,
                weeklySchedule.IsUsedWeeklyDefaultShift
                    ? GetDefaultShift(weeklyDefaultShift, weeklySchedule.ThursdaySchedule.WorkDate.DayOfWeek) : defaultShift);
            MappingDailyDefaultShift(
                weeklySchedule.FridaySchedule,
                weeklySchedule.IsUsedWeeklyDefaultShift
                    ? GetDefaultShift(weeklyDefaultShift, weeklySchedule.FridaySchedule.WorkDate.DayOfWeek) : defaultShift);
            MappingDailyDefaultShift(
                weeklySchedule.SaturdaySchedule,
                weeklySchedule.IsUsedWeeklyDefaultShift
                    ? GetDefaultShift(weeklyDefaultShift, weeklySchedule.SaturdaySchedule.WorkDate.DayOfWeek) : defaultShift);
            MappingDailyDefaultShift(
                weeklySchedule.SundaySchedule,
                weeklySchedule.IsUsedWeeklyDefaultShift
                    ? GetDefaultShift(weeklyDefaultShift, weeklySchedule.SundaySchedule.WorkDate.DayOfWeek) : defaultShift);
        }
    }

    private static void MappingDefaultShift(DailySchedule dailySchedule, ShiftBase defaultShift)
    {
        if (dailySchedule.Shift == null && defaultShift != null)
        {
            if (defaultShift.Id == Constants.OffDayShiftId)
            {
                dailySchedule.IsOffDay = true;
            }
            else
            {
                dailySchedule.Shift = defaultShift;
            }
        }
    }

    private static ShiftBase GetDefaultShift(WeeklyDefaultShift weeklyDefaultShift, DayOfWeek dayOfWeek)
    {
        ShiftBase shift = null;
        if (weeklyDefaultShift != null)
        {
            shift = dayOfWeek switch
            {
                DayOfWeek.Monday => weeklyDefaultShift.MonShift,
                DayOfWeek.Tuesday => weeklyDefaultShift.TueShift,
                DayOfWeek.Wednesday => weeklyDefaultShift.WedShift,
                DayOfWeek.Thursday => weeklyDefaultShift.ThuShift,
                DayOfWeek.Friday => weeklyDefaultShift.FriShift,
                DayOfWeek.Saturday => weeklyDefaultShift.SatShift,
                DayOfWeek.Sunday => weeklyDefaultShift.SunShift,
                _ => null
            };
        }

        return shift;
    }

    private static void MappingDailyDefaultShift(DailySchedule dailySchedule, ShiftBase defaultShift)
    {
        if (dailySchedule.IsModified || dailySchedule.IsLeaveOffDay || (dailySchedule.IsLeaveDay && !dailySchedule.IsLeaveHalfDay))
        {
            return;
        }

        if (dailySchedule.IsHoliday || dailySchedule.IsOffDay || dailySchedule.IsChangeShift || dailySchedule.IsOvertime)
        {
            return;
        }

        MappingDefaultShift(dailySchedule, defaultShift);
    }
}