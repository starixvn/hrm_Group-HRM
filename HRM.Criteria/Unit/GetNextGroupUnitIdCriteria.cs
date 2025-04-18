namespace HRM.Criteria.Unit;

using Fanex.Data.Repository;

public class GetNextGroupUnitIdCriteria : CriteriaBase
{
    public override string GetSettingKey() => "Unit_GetNextGroupUnitId";

    public override bool IsValid() => true;
}