namespace HRM.Criteria.Request.LateInEarlyOut;

using Fanex.Data.Repository;

public class DeleteLateInEarlyOutRequestCriteria : CriteriaBase
{
    public long RequestId { get; set; }

    public override string GetSettingKey() => "LateInEarlyOutRequest_Delete";

    public override bool IsValid() => RequestId > 0;
}