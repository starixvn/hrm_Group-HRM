namespace HRM.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetDisciplineSummaryPerYearCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string TeamId { get; set; }

        public int? EmployeeId { get; set; }

        public bool? IsLocal { get; set; }

        public int ApproverId { get; set; }

        public bool IsShowAll { get; set; }

        public override bool IsValid() => FromDate <= ToDate && ApproverId > 0;

        public override string GetSettingKey() => "Report_DisciplineSummaryPerYear";
    }
}