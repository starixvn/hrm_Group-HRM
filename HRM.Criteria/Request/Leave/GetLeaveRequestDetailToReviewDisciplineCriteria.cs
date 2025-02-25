namespace HRM.Criteria.Request.Leave
{
    using Fanex.Data.Repository;

    public class GetLeaveRequestDetailToReviewDisciplineCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey() => "LeaveRequest_GetDetailToReviewDiscipline";

        public override bool IsValid() => RequestId > 0;
    }
}