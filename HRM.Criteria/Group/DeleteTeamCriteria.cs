namespace HRM.Criteria.Group
{
    using Fanex.Data.Repository;

    public class DeleteTeamCriteria : CriteriaBase
    {
        public int TeamId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Group_Delete";
        }

        public override bool IsValid()
        {
            return TeamId > 0;
        }
    }
}