namespace HRM.Criteria.Request.ChangeShift
{
    using Fanex.Data.Repository;

    public class GetChangeShiftDetailToReviewDisciplineCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey() => "ChangeShiftRequest_GetDetailToReviewDiscipline";

        public override bool IsValid() => RequestId > 0;
    }
}