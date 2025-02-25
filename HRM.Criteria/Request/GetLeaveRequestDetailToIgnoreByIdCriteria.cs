namespace HRM.Criteria.Request.Leave
{
    using Fanex.Data.Repository;

    public class GetLeaveRequestDetailToIgnoreByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveRequest_GetDetailToIgnore";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}