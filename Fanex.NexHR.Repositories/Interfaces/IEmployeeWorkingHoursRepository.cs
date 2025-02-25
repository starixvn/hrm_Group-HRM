namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Report.MonthlyWorkingHours;

    public interface IEmployeeWorkingHoursRepository
    {
        IEnumerable<MonthlyWorking> GetMonthlyWorkingHours(ICriteria criteria);
    }
}