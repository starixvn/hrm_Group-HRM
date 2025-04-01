namespace HRM.Criteria.Request.LateInEarlyOut;

using Fanex.Data.Repository;

public class GetLateInEarlyOutRequestHistoryByIdCriteria : CriteriaBase
{
    public long RequestId { get; set; }

    public override string GetSettingKey() => "LateInEarlyOutRequest_GetHistoryById";

    public override bool IsValid() => RequestId > 0;
}