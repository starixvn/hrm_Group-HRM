namespace Fanex.NexHR.Criteria.Request.CheckInOut
{
    using Fanex.Data.Repository;

    public class GetCheckInOutRequestByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "CheckInOutRequest_GetById";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
