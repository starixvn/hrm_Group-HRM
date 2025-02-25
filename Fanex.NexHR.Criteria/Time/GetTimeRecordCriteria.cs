namespace Fanex.NexHR.Criteria.Time
{
    using System;
    using Fanex.Data.Repository;

    public class GetTimeRecordCriteria : CriteriaBase
    {
        public int? EmpId { get; set; }
        public byte? IsLocal { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string TeamIds { get; set; }
        public int? PageIndex { get; set; }
        public int? ItemsPerPage { get; set; }
        public int? TotalItem { get; set; }
        public int SupervisorId { get; set; }

        public override string GetSettingKey() => "TimeRecord_Get";

        public override bool IsValid() => FromDate <= ToDate;
    }
}