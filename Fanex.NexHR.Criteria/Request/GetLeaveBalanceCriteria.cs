namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;

    public class GetLeaveBalanceCriteria : CriteriaBase
    {
        public int EmpId { get; set; }
        public long? RequestId { get; set; }
        public DateTime? EffectedDay { get; set; }

        public override string GetSettingKey() => "LeaveRequest_GetLeaveBalance";

        public override bool IsValid() => EmpId > 0;
    }
}
