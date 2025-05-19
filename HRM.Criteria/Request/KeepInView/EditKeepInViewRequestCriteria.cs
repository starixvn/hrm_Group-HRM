namespace HRM.Criteria.Request.KeepInView;

using System;
using Fanex.Data.Repository;

public class EditKeepInViewRequestCriteria : CriteriaBase
{
    public long RequestId { get; set; }
    public int EmpId { get; set; }
    public DateTime OnDate { get; set; }
    public byte KeepInViewType { get; set; }
    public bool IsHalfDay { get; set; }
    public DateTime FromTime { get; set; }
    public DateTime ToTime { get; set; }
    public string Remark { get; set; }
    public int ModifiedBy { get; set; }
    public int? VerifierId { get; set; }
    public int? ApproverId { get; set; }
    public short CompanyId { get; set; }

    public override string GetSettingKey() => "KeepInViewRequest_Update";

    public override bool IsValid() => EmpId > 0 && ModifiedBy > 0;
}