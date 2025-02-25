namespace Fanex.NexHR.Criteria.Group
{
    using Fanex.Data.Repository;

    public class EditDisciplineRuleCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public int TeamId { get; set; }

        public string DisciplineRuleXml { get; set; }

        public override bool IsValid() => TeamId > 0 && ActorId > 0;

        public override string GetSettingKey() => "DisciplineRule_Update";
    }
}