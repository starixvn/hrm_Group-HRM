namespace HRM.Criteria.Supervisor;

using Fanex.Data.Repository;

public class GetSupervisorByTeamIdCriteria : CriteriaBase
{
    public int TeamId { get; set; }

    public bool IsGroupSpace { get; set; }

    public int? CompanyId { get; set; }

    public override bool IsValid() => TeamId > 0;

    public override string GetSettingKey() => "GroupSupervisor_GetById";
}