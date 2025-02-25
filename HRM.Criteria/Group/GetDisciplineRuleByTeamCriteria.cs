namespace HRM.Criteria.Group
{
    using Fanex.Data.Repository;

    public class GetDisciplineRuleByTeamCriteria : CriteriaBase
    {
        public int TeamId { get; set; }

        public override string GetSettingKey() => "DisciplineRule_GetByGroupId";

        public override bool IsValid() => TeamId > 0;
    }
}