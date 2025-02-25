namespace HRM.Criteria.Lookup
{
    using Fanex.Data.Repository;

    public class UpdateLookupDataCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CategoryId { get; set; }

        public override string GetSettingKey()
        {
            return "LookupData_Update";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
