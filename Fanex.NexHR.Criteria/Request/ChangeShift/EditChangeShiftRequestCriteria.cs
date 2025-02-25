namespace Fanex.NexHR.Criteria.Request.ChangeShift
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class EditChangeShiftRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public byte TypeId { get; set; }
        public int? VerifierId { get; set; }
        public int? ApproverId { get; set; }
        public string Remark { get; set; }
        public string DetailXml { get; set; }
        public int ActorId { get; set; }

        public override string GetSettingKey() => "ChangeShiftRequest_Update";

        public override bool IsValid() => RequestId > 0 && EmployeeId > 0 && FromDate <= ToDate && ActorId > 0;
    }
}