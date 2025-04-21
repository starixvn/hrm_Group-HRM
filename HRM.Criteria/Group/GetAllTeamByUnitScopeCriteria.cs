using Fanex.Data.Repository;

namespace HRM.Criteria.Group;

public class GetAllTeamByUnitScopeCriteria : CriteriaBase
{
    public string Keyword { get; set; }

    public override string GetSettingKey() => "AllGroup_UnitScope_Get";

    public override bool IsValid() => true;
}