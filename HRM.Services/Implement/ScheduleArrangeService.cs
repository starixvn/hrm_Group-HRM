namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Core.XMLExtensions;
    using HRM.Criteria.ScheduleArrange;
    using HRM.Models;
    using HRM.Models.ScheduleArrange;
    using HRM.Repositories;

    public class ScheduleArrangeService : IScheduleArrangeService
    {
        private readonly IScheduleArrangeRepository scheduleArrangeRepository;
        private readonly IBaseService baseService;
        private readonly IShiftService shiftService;

        public ScheduleArrangeService(
            IScheduleArrangeRepository scheduleArrangeRepository,
            IBaseService baseService,
            IShiftService shiftService)
        {
            this.scheduleArrangeRepository = scheduleArrangeRepository;
            this.baseService = baseService;
            this.shiftService = shiftService;
        }

        public async Task<IEnumerable<EmployeeSuggestion>> GetEmployeesAsync(GetEmployeeFilter filter)
        {
            var criteria = new GetEmployeeCriteria
            {
                ActorId = filter.ActorId,
                TeamIds = filter.TeamIds,
                Query = filter.Query,
                IsIFrame = filter.IsIFrame
            };

            return await baseService.GetDataAsync<EmployeeSuggestion>(criteria);
        }

        public async Task<IEnumerable<WeeklySchedule>> GetWeeklyScheduleAsync(WeeklyScheduleFilter filter)
        {
            bool? isLocal = null;
            var criteria = new GetWeeklyScheduleCriteria
            {
                ActorId = filter.ActorId,
                FromTime = filter.FromDate,
                ToTime = filter.ToDate,
                EmpId = filter.EmployeeId,
                TeamIds = filter.TeamIds,
                IsIFrame = filter.IsIFrame,
                IsLocal = filter.IsLocal.HasValue ? (filter.IsLocal.Value == 1) : isLocal
            };

            IEnumerable<WeeklySchedule> weeklySchedules = await scheduleArrangeRepository.GetWeeklyScheduleAsync(criteria);
            //IList<Shift> shifts = await shiftService.GetAllAsync(isShowOff: true);
            //MappingWeeklyDefaultShift(weeklySchedules, shifts);
            //MappingDefaultShift(weeklySchedules);

            return weeklySchedules;
        }

        public async Task<CommandResult> EditWeeklyAsync(
            int actorId,
            DateTime fromDate,
            DateTime toDate,
            IEnumerable<EditDailySchedule> weeklySchedules,
            IEnumerable<EditDefaultShift> defaultShifts)
        {
            var criteria = new EditWeeklyScheduleCriteria
            {
                ActorId = actorId,
                FromDate = fromDate,
                ToDate = toDate,
                WeeklyScheduleXml = weeklySchedules.ToXML().ToString(),
                DefaultShiftXml = defaultShifts.ToXML().ToString()
            };

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
            var criteria = new EditWeeklyDefaultShiftCriteria
            {
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
            return shiftId.HasValue && shiftId.Value != Constants.OffDayShiftId ? shiftId.Value : (int?)null;
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
            foreach (var weeklyDefaultShift in weeklyDefaultShifts)
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
            foreach (WeeklySchedule weeklySchedule in weeklySchedules)
            {
                WeeklyDefaultShift weeklyDefaultShift = weeklySchedule.WeeklyDefaultShift;
                ShiftBase defaultShift = weeklySchedule.DefaultShift;

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
                switch (dayOfWeek)
                {
                    case DayOfWeek.Monday:
                        shift = weeklyDefaultShift.MonShift;
                        break;

                    case DayOfWeek.Tuesday:
                        shift = weeklyDefaultShift.TueShift;
                        break;

                    case DayOfWeek.Wednesday:
                        shift = weeklyDefaultShift.WedShift;
                        break;

                    case DayOfWeek.Thursday:
                        shift = weeklyDefaultShift.ThuShift;
                        break;

                    case DayOfWeek.Friday:
                        shift = weeklyDefaultShift.FriShift;
                        break;

                    case DayOfWeek.Saturday:
                        shift = weeklyDefaultShift.SatShift;
                        break;

                    case DayOfWeek.Sunday:
                        shift = weeklyDefaultShift.SunShift;
                        break;

                    default:
                        // nothing
                        break;
                }
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
}