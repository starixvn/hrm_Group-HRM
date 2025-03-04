namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.Models.Report.Overtime;

    public interface IOvertimeReportService
    {
        IEnumerable<YearlyOvertime> GetYearlyOvertime(GetYearlyOvertimeParam param);

        Task<IEnumerable<DailyOvertimeDetails>> GetMonthlyOvertimeDetailsAsync(GetMonthlyOvertimeDetailParam param);

        Task<Tuple<IEnumerable<DateRange>, IEnumerable<DailyOvertimeDetails>>> GetMonthlyOvertimeAsync(GetMonthlyOvertimeParam param);

        Task<IEnumerable<DailyOvertimeDetails>> GetDailyOvertimeDetailsAsync(GetDailyOvertimeDetailsParam param);

        byte[] ExportYearlyOvertime(IEnumerable<YearlyOvertime> overtimes, GetYearlyOvertimeParam param);

        byte[] ExportMonthlyOvertime(IEnumerable<DateRange> dateRanges, IEnumerable<DailyOvertimeDetails> dailyOvertimes);

        byte[] ExportDailyOvertimeDetails(IEnumerable<DailyOvertimeDetails> overtimes, GetDailyOvertimeDetailsParam param);
    }
}