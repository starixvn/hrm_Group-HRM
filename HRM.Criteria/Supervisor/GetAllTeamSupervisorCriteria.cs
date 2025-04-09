namespace HRM.Criteria.Supervisor;

using Fanex.Data.Repository;

public class GetTeamSupervisorCriteria : CriteriaBase
{
    public short CompanyId { get; set; }
    public string TeamIds { get; set; }

    public override bool IsValid() => true;

    public override string GetSettingKey() => "GroupSupervisor_Get";
}