using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using HRM.Models.ScheduleArrange;

namespace HRM.Repositories.Interfaces;

public interface IScheduleArrangeRepository
{
    Task<List<WeeklySchedule>> GetWeeklyScheduleAsync(ICriteria criteria);
}