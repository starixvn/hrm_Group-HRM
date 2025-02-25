namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduleArrange;

    public interface IScheduleArrangeService
    {
        IEnumerable<EmployeeSuggestion> GetEmployees(GetEmployeeFilter filter);

        IEnumerable<WeeklySchedule> GetWeeklySchedule(WeeklyScheduleFilter filter);

        CommandResult EditWeekly(
            int actorId,
            DateTime fromDate,
            DateTime toDate,
            IEnumerable<EditDailySchedule> weeklySchedules,
            IEnumerable<EditDefaultShift> defaultShifts);

        CommandResult EditRemark(int actorId, EditRemark model);

        CommandResult EditDisplayOrder(int actorId, IEnumerable<EditDisplayOrder> displayOrders);

        CommandResult EditTeamOrder(int actorId, IEnumerable<EditTeamOrder> displayOrders);

        CommandResult EditWeeklyDefaultShift(int actorId, WeeklyDefaultShift weeklyDefaultShift, bool isUsedWeeklyDefaultShift);
    }
}