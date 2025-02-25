namespace HRM.Criteria.Request.ChangeShift
{
    using Fanex.Data.Repository;

    public class GetChangeShiftRequestByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey() => "ChangeShiftRequest_GetById";

        public override bool IsValid() => RequestId > 0;
    }
}