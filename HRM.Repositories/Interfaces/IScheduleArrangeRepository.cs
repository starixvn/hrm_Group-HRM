namespace HRM.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Models.ScheduleArrange;

public interface IScheduleArrangeRepository
{
    Task<IEnumerable<WeeklySchedule>> GetWeeklyScheduleAsync(ICriteria criteria);
}