namespace Fanex.NexHR.Criteria.EmployeeRelationship
{
    using Fanex.Data.Repository;

    public class GetRelationshipByIdCriteria : CriteriaBase
    {
        public int RelationshipId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeRelationship_GetById";
        }

        public override bool IsValid()
        {
            return RelationshipId > 0;
        }
    }
}