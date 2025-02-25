namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetMonthlyOvertimeDetailsReportCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Report_MonthlyOvertimeDetails";

        public override bool IsValid() => EmployeeId > 0;
    }
}