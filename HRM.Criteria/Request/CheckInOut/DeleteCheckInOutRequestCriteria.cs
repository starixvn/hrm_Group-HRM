namespace HRM.Criteria.Request.CheckInOut;

using Fanex.Data.Repository;

public class DeleteCheckInOutRequestCriteria : CriteriaBase
{
    public long RequestId { get; set; }

    public override string GetSettingKey() => "CheckInOutRequest_Delete";

    public override bool IsValid() => RequestId > 0;
}