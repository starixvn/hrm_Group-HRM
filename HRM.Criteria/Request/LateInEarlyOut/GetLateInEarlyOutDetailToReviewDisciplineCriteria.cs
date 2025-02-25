namespace HRM.Criteria.Request.LateInEarlyOut
{
    using Fanex.Data.Repository;

    public class GetLateInEarlyOutDetailToReviewDisciplineCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey() => "LateInEarlyOutRequest_GetDetailToReviewDiscipline";

        public override bool IsValid() => RequestId > 0;
    }
}