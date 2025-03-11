namespace HRM.Criteria.Calendar
{
    using System;
    using Fanex.Data.Repository;

    public class GetWorkingCalendarWeeklyCriteria : CriteriaBase
    {
        public int? EmpId { get; set; }
        public DateTime Fromtime { get; set; }
        public DateTime Totime { get; set; }
        public string TeamIds { get; set; }
        public string JobTitleId { get; set; }
        public byte? IsLocal { get; set; }
        public short CompanyId { get; set; }

        public override string GetSettingKey() => "Report_EmployeeCalendarByWeekly";

        public override bool IsValid() => true;
    }
}