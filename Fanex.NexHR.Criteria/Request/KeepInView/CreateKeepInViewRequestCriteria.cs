namespace Fanex.NexHR.Criteria.Request.KeepInView
{
    using System;
    using Fanex.Data.Repository;

    public class CreateKeepInViewRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int EmpId { get; set; }
        public string Remark { get; set; }
        public byte KeepInViewType { get; set; }
        public DateTime OnDate { get; set; }
        public bool IsHalfDay { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public int CreatedBy { get; set; }
        public int? VerifierId { get; set; }
        public int? ApproverId { get; set; }
        public int? ShiftId { get; set; }

        public override string GetSettingKey() => "KeepInViewRequest_Insert";

        public override bool IsValid() => EmpId > 0 && CreatedBy > 0;
    }
}