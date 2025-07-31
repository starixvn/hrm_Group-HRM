using Fanex.Data.Repository;

namespace HRM.Criteria.Group;

public class GetDisciplineManagerByTeamIdCriteria : CriteriaBase
{
    public int TeamId { get; set; }

    public short? CompanyId { get; set; }

    public override bool IsValid() => TeamId > 0;

    public override string GetSettingKey() => "GroupSupervisor_GetDisciplineSettingById";
}