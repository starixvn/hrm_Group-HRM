namespace HRM.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRM.Models.Report.Overtime;

    public interface IOvertimeReportRepository
    {
        Task<Tuple<IEnumerable<DateRange>, IEnumerable<DailyOvertimeDetails>>> GetMonthlyOvertimeAsync(ICriteria criteria);
    }
}