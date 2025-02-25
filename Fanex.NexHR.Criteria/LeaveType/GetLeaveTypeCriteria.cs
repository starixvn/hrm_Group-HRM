namespace Fanex.NexHR.Criteria.LeaveType
{
    using Fanex.Data.Repository;

    public class GetLeaveTypeCriteria : CriteriaBase
    {
        public bool ExcludeNotShown { get; set; }

        public override string GetSettingKey() => "LeaveType_GetAll";

        public override bool IsValid() => true;
    }
}