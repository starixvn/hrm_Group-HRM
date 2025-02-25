namespace Fanex.NexHR.Criteria.Request.CheckInOut
{
    using Fanex.Data.Repository;

    public class GetCheckInOutDetailToReviewDisciplineCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey() => "CheckInOutRequest_GetDetailToReviewDiscipline";

        public override bool IsValid() => RequestId > 0;
    }
}