namespace HRM.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Models.Report.MonthlyWorkingHours;

public interface IEmployeeWorkingHoursRepository
{
    Task<IEnumerable<MonthlyWorking>> GetMonthlyWorkingHoursAsync(ICriteria criteria);
}