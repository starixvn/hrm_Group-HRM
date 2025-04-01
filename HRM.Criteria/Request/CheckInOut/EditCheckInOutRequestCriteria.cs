namespace HRM.Criteria.Request.CheckInOut;

using System;
using Fanex.Data.Repository;

public class EditCheckInOutRequestCriteria : CriteriaBase
{
    public long RequestId { get; set; }

    public int EmpId { get; set; }

    public byte RequestType { get; set; }

    public string Remark { get; set; }

    public DateTime OnDate { get; set; }

    public DateTime RequestTime { get; set; }

    public byte ReasonId { get; set; }

    public int? VerifierId { get; set; }

    public int? ApproverId { get; set; }

    public int ModifiedBy { get; set; }

    public string RefNo { get; set; }

    public override string GetSettingKey() => "CheckInOutRequest_Update";

    public override bool IsValid() => EmpId > 0 && ReasonId > 0 && (RequestTime - OnDate).Days >= 0 && (RequestTime - OnDate).Days <= 1;
}