namespace Fanex.NexHR.Criteria.EmployeeRelationship
{
    using Fanex.Data.Repository;

    public class GetRelationshipByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeRelationship_GetByEmployeeId";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}