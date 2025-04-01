namespace HRM.Criteria.Unit;

using Fanex.Data.Repository;

public class GetUnitsCriteria : CriteriaBase
{
    public bool? IsActive { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey() => "Unit_Get";

    public override bool IsValid() => true;
}
