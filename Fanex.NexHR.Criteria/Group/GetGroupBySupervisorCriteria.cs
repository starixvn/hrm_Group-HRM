namespace Fanex.NexHR.Criteria.Group
{
    using Fanex.Data.Repository;

    public class GetGroupBySupervisorCriteria : CriteriaBase
    {
        public int SupervisorId { get; set; }

        public override string GetSettingKey() => "Group_GetBySupervisor";

        public override bool IsValid() => SupervisorId > 0;
    }
}