namespace HRM.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRM.Models.Report.Overtime;

    public class OvertimeReportRepository : IOvertimeReportRepository
    {
        public async Task<Tuple<IEnumerable<DateRange>, IEnumerable<DailyOvertimeDetails>>> GetMonthlyOvertimeAsync(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    var dateRanges = multi.Read<DateRange>();
                    var dailyOvertimes = (multi.Read<DailyOvertimeDetails>()).OrderBy(x => x.WorkDate).ThenByDescending(x => x.Team);

                    return new Tuple<IEnumerable<DateRange>, IEnumerable<DailyOvertimeDetails>>(dateRanges, dailyOvertimes);
                }
            }
        }
    }
}