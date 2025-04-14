namespace HRM.Criteria.Unit;

using Fanex.Data.Repository;

public class GetUnitCriteria : CriteriaBase
{
    public bool? IsActive { get; set; }

    public override string GetSettingKey() => "Unit_Get";

    public override bool IsValid() => true;
}
