namespace HRM.Criteria.Lookup
{
    using Fanex.Data.Repository;

    public class GetJobTitleCriteria : CriteriaBase
    {
        public string JobTitle { get; set; }

        public int CompanyId { get; set; }

        public override string GetSettingKey() => "Lookup_GetJobTitle";

        public override bool IsValid() => true;
    }
}