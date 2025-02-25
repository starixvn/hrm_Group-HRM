namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Report.Overtime;

    public interface IOvertimeReportRepository
    {
        Tuple<IEnumerable<DateRange>, IEnumerable<DailyOvertimeDetails>> GetMonthlyOvertime(ICriteria criteria);
    }
}