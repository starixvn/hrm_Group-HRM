namespace HRM.Criteria.EmployeeRelationship
{
    using Fanex.Data.Repository;

    public class DeleteRelationshipCriteria : CriteriaBase
    {
        public int RelationshipId { get; set; }

        public int ActorId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeRelationship_Delete";
        }

        public override bool IsValid()
        {
            return RelationshipId > 0 && ActorId > 0;
        }
    }
}