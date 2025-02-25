namespace Fanex.NexHR.Criteria.Request
{
    using Fanex.Data.Repository;

    public class GetRequestRelatedPeopleCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int Action { get; set; }

        public override string GetSettingKey() => "EmployeeSupervisor_GetListSupervisorRequestID";

        public override bool IsValid() => RequestId > 0;
    }
}