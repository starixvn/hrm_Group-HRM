namespace HRM.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using HRM.Models.Report.MonthlyWorkingHours;

    public interface IEmployeeWorkingHoursRepository
    {
        IEnumerable<MonthlyWorking> GetMonthlyWorkingHours(ICriteria criteria);
    }
}