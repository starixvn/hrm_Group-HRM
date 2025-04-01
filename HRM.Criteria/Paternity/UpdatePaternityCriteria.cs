namespace HRM.Criteria.Paternity;

using System;
using Fanex.Data.Repository;

public class UpdatePaternityCriteria : CriteriaBase
{
    public int PaternityId { get; set; }

    public int EmpId { get; set; }

    public DateTime ChildBornDate { get; set; }

    public int LeaveDay { get; set; }

    public int ModifiedBy { get; set; }

    public string Remark { get; set; }

    public override string GetSettingKey() => "Paternity_Update";

    public override bool IsValid() => PaternityId > 0 && EmpId > 0 && ModifiedBy > 0 && LeaveDay > 0;
}