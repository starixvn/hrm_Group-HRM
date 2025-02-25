namespace HRM.Criteria.Section
{
    using Fanex.Data.Repository;

    public class GetParentBySectionCriteria : CriteriaBase
    {
        public int SectionId { get; set; }

        public override string GetSettingKey() => "Section_GetParent";

        public override bool IsValid() => SectionId > 0;
    }
}