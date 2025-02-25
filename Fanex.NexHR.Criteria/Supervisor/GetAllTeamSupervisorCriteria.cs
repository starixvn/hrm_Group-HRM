namespace Fanex.NexHR.Criteria.Supervisor
{
    using Fanex.Data.Repository;

    public class GetTeamSupervisorCriteria : CriteriaBase
    {
        public string TeamIds { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "GroupSupervisor_Get";
        }
    }
}
