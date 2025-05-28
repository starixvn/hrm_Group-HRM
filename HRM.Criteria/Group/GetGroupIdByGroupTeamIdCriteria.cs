namespace HRM.Criteria.Group;

using Fanex.Data.Repository;

public class GetGroupIdByGroupTeamIdCriteria : CriteriaBase
{
    public int GroupTeamId { get; set; }

    public override bool IsValid() => true;

    public override string GetSettingKey() => "GroupId_ByGroupTeamId_Get";
}