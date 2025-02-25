namespace HRM.Criteria.Discipline
{
    using Fanex.Data.Repository;

    public class UpdateIgnoredDisciplineCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public string IgnoreDetailXml { get; set; }

        public string DisciplineIgnoreXml { get; set; }

        public bool HasUpdateOutOfControlReason { get; set; }

        public override string GetSettingKey() => "Discipline_UpdateIgnore";

        public override bool IsValid() => ActorId > 0 && !string.IsNullOrWhiteSpace(IgnoreDetailXml) && !string.IsNullOrWhiteSpace(DisciplineIgnoreXml);
    }
}