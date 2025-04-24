using Fanex.Data.Repository;

namespace HRM.Criteria.Group;

public class GetTeamInUnitScopeCriteria : CriteriaBase
{
    public int? TeamId { get; set; }

    public string Keyword { get; set; }

    public override string GetSettingKey() => "Group_UnitScope_Get";

    public override bool IsValid() => true;
}