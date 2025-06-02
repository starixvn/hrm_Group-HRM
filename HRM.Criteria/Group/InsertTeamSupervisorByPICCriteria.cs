namespace HRM.Criteria.Group;

using Fanex.Data.Repository;

public class InsertTeamSupervisorByPICCriteria : CriteriaBase
{
    public int TeamId { get; set; }
    public string SupervisorXml { get; set; }
    public int ActorId { get; set; }

    public override bool IsValid() => TeamId > 0 && ActorId > 0;

    public override string GetSettingKey() => "GroupSupervisor_ByPIC_Insert";
}