namespace HRM.Criteria.Postnatal;

using System;
using Fanex.Data.Repository;

public class InsertPostnatalCriteria : CriteriaBase
{
    public int EmpId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int CreatedBy { get; set; }
    public string Remark { get; set; }
    public byte WorkingHours { get; set; }

    public override string GetSettingKey() => "Postnatal_Insert";

    public override bool IsValid() => EmpId > 0 && FromDate <= ToDate && CreatedBy > 0;
}