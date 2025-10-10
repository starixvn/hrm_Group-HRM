using Fanex.Data.Repository;

namespace HRM.Criteria.Group;

public class GetLeaderTakeoverByTeamIdCriteria : CriteriaBase
{
    public int TeamId { get; set; }

    public short? CompanyId { get; set; }

    public override bool IsValid() => TeamId > 0;

    public override string GetSettingKey() => "GroupSupervisor_GetLeaderTakeoverByTeamId";
}
