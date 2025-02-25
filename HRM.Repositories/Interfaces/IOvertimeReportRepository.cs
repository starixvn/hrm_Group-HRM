namespace HRM.Repositories
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using HRM.Models.Report.Overtime;

    public interface IOvertimeReportRepository
    {
        Tuple<IEnumerable<DateRange>, IEnumerable<DailyOvertimeDetails>> GetMonthlyOvertime(ICriteria criteria);
    }
}