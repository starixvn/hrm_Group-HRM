namespace Fanex.NexHR.Criteria.Lookup
{
    using Fanex.Data.Repository;

    public class DeleteLookupDataCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "LookupData_Delete";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
