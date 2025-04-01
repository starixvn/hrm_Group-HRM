using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using HRM.Models.Report.MonthlyWorkingHours;

namespace HRM.Repositories.Interfaces;

public interface IEmployeeWorkingHoursRepository
{
    Task<IEnumerable<MonthlyWorking>> GetMonthlyWorkingHoursAsync(ICriteria criteria);
}