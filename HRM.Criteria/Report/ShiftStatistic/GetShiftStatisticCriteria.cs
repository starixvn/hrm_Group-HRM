namespace HRM.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetShiftStatisticCriteria : CriteriaBase
    {
        public int SupervisorId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string TeamIds { get; set; }

        public bool IsShowWholeCompany { get; set; }
        public byte? IsLocal { get; set; }

        public short CompanyId { get; set; }

        public override string GetSettingKey() => "Report_ShiftStatistic";

        public override bool IsValid() => SupervisorId > 0;
    }
}