namespace Fanex.NexHR.Criteria.Request.LateInEarlyOut
{
    using Fanex.Data.Repository;

    public class GetLateInEarlyOutRequestByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey() => "LateInEarlyOutRequest_GetById";

        public override bool IsValid() => RequestId > 0;
    }
}