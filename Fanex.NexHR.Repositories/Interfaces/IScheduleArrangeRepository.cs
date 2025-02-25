namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.ScheduleArrange;

    public interface IScheduleArrangeRepository
    {
        IEnumerable<WeeklySchedule> GetWeeklySchedule(ICriteria criteria);
    }
}