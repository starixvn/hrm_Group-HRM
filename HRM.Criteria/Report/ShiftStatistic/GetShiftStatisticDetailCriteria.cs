namespace HRM.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetShiftStatisticDetailCriteria : CriteriaBase
    {
        public int SupervisorId { get; set; }
        public int? EmployeeId { get; set; }
        public int? ShiftId { get; set; }
        public DateTime WorkDate { get; set; }
        public string TeamIds { get; set; }

        public bool IsShowWholeCompany { get; set; }
        public byte? IsLocal { get; set; }

        public override string GetSettingKey() => "Report_ShiftStatisticDetail";

        public override bool IsValid() => SupervisorId > 0;
    }
}