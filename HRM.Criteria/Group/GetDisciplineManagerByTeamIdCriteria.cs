namespace HRM.Group
{
    using Fanex.Data.Repository;

    public class GetDisciplineManagerByTeamIdCriteria : CriteriaBase
    {
        public int TeamId { get; set; }

        public override bool IsValid() => TeamId > 0;

        public override string GetSettingKey() => "GroupSupervisor_GetDisciplineSettingById";
    }
}