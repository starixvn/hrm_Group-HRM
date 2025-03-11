namespace HRM.Criteria.Section
{
    using Fanex.Data.Repository;

    public class GetSectionCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public short CompanyId { get; set; }

        public override string GetSettingKey() => "Section_Get";

        public override bool IsValid() => true;
    }
}
