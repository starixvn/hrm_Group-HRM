namespace HRM.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetYearlyOvertimeReportCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public string TeamIds { get; set; }
        public string Employee { get; set; }
        public byte? EmployeeStatusId { get; set; }
        public string MonthXml { get; set; }
        public byte? IsLocal { get; set; }

        public override string GetSettingKey() => "Report_YearlyOvertime";

        public override bool IsValid() => ActorId > 0;
    }
}