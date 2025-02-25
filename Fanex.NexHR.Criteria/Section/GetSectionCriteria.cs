namespace Fanex.NexHR.Criteria.Section
{
    using Fanex.Data.Repository;

    public class GetSectionCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "Section_Get";

        public override bool IsValid() => true;
    }
}
