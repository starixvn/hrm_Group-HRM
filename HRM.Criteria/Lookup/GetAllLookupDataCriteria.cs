namespace HRM.Criteria.Lookup
{
    using Fanex.Data.Repository;

    public class GetAllLookupDataCriteria : CriteriaBase
    {
        public string LookupCode { get; set; }

        public override string GetSettingKey() => "Lookup_GetLookupData";

        public override bool IsValid() => true;
    }
}