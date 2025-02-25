
namespace Fanex.NexHR.Criteria.Report.MealAllowance
{
    using System;
    using Fanex.Data.Repository;

    public class GetMealAllowanceSummaryReportCriteria : CriteriaBase
    {
        public int? EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string TeamId { get; set; }
        public byte? IsLocal { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "WorkFromHome_MealAllowanceSummaryReport";
    }
}
