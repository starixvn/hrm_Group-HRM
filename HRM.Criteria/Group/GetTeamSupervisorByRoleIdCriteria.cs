namespace HRM.Criteria.Group;

using Fanex.Data.Repository;

public class GetTeamSupervisorByRoleIdCriteria : CriteriaBase
{
    public int TeamId { get; set; }
    public int Role { get; set; }

    public override bool IsValid() => TeamId > 0;

    public override string GetSettingKey() => "GroupSupervisor_GetByRole";
}