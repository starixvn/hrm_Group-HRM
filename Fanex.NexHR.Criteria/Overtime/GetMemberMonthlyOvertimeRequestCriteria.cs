namespace Fanex.NexHR.Criteria.Overtime
{
    using System;
    using Fanex.Data.Repository;

    public class GetMemberMonthlyOvertimeRequestCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int? EmployeeId { get; set; }
        public string TeamIds { get; set; }
        public string StatusIds { get; set; }
        public DateTime? Month { get; set; }
        public bool IsProcessor { get; set; }
        public short OvertimeType { get; set; }
        public byte? IsLocal { get; set; }

        public override string GetSettingKey() => "Overtime_GetMemberMonthlyRequest";

        public override bool IsValid() => true;
    }
}