namespace HRM.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using HRM.Models.ScheduleArrange;

    public interface IScheduleArrangeRepository
    {
        IEnumerable<WeeklySchedule> GetWeeklySchedule(ICriteria criteria);
    }
}