namespace Fanex.NexHR.Criteria.HospitalizationLeave
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetSummaryReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int? EmployeeId { get; set; }

        public string TeamIds { get; set; }
        public int Year { get; set; }
        public bool? IsLocal { get; set; }
        public int? SupervisorId { get; set; }

        public override string GetSettingKey() => "HospitalizationLeave_SummaryReport";

        public override bool IsValid() => true;
    }
}