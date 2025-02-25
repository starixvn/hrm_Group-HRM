namespace HRM.Criteria.Request.CheckInOut
{
    using Fanex.Data.Repository;

    public class GetCheckInOutRequestHistoryByIdCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "CheckInOutRequest_GetHistoryById";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
