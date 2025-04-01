namespace HRM.Criteria.Request.KeepInView;

using Fanex.Data.Repository;

public class GetKeepInViewRequestHistoryByIdCriteria : CriteriaBase
{
    public long RequestId { get; set; }

    public override string GetSettingKey()
    {
        return "KeepInViewRequest_GetHistoryById";
    }

    public override bool IsValid()
    {
        return RequestId > 0;
    }
}