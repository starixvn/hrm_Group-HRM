using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Criteria.ScheduleArrange;
using HRM.Models;
using HRM.Models.ScheduleArrange;

namespace HRM.Services.Interfaces;

public interface IScheduleArrangeService
{
    Task<IEnumerable<EmployeeSuggestion>> GetEmployeesAsync(GetEmployeeFilter filter);

    Task<IEnumerable<WeeklySchedule>> GetWeeklyScheduleAsync(WeeklyScheduleFilter filter);

    Task<CommandResult> EditWeeklyAsync(EditWeeklyScheduleCriteria criteria);

    Task<CommandResult> EditRemarkAsync(int actorId, EditRemark model);

    Task<CommandResult> EditDisplayOrderAsync(int actorId, IEnumerable<EditDisplayOrder> displayOrders);

    Task<CommandResult> EditTeamOrderAsync(int actorId, IEnumerable<EditTeamOrder> displayOrders);

    Task<CommandResult> EditWeeklyDefaultShiftAsync(int actorId, WeeklyDefaultShift weeklyDefaultShift, bool isUsedWeeklyDefaultShift);
}