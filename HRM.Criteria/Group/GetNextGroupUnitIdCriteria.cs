namespace HRM.Criteria.Group;

using Fanex.Data.Repository;

public class GetNextGroupTeamIdCriteria : CriteriaBase
{
    public override string GetSettingKey() => "Group_GetNextGroupTeamId";

    public override bool IsValid() => true;
}