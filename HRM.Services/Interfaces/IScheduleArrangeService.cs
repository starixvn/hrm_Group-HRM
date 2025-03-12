namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.Core.Models;
    using Models;
    using Models.ScheduleArrange;

    public interface IScheduleArrangeService
    {
        Task<IEnumerable<EmployeeSuggestion>> GetEmployeesAsync(GetEmployeeFilter filter);
        Task<IEnumerable<WeeklySchedule>> GetWeeklyScheduleAsync(WeeklyScheduleFilter filter);
        Task<CommandResult> EditWeeklyAsync(int actorId, DateTime fromDate, DateTime toDate, IEnumerable<EditDailySchedule> weeklySchedules, IEnumerable<EditDefaultShift> defaultShifts);
        Task<CommandResult> EditRemarkAsync(int actorId, EditRemark model);
        Task<CommandResult> EditDisplayOrderAsync(int actorId, IEnumerable<EditDisplayOrder> displayOrders);
        Task<CommandResult> EditTeamOrderAsync(int actorId, IEnumerable<EditTeamOrder> displayOrders);
        Task<CommandResult> EditWeeklyDefaultShiftAsync(int actorId, WeeklyDefaultShift weeklyDefaultShift, bool isUsedWeeklyDefaultShift);
    }
}