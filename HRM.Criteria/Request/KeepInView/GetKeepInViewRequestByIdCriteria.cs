namespace HRM.Criteria.Request.KeepInView;

using Fanex.Data.Repository;

public class GetKeepInViewRequestByIdCriteria : CriteriaBase
{
    public long RequestId { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey()
    {
        return "KeepInViewRequest_GetById";
    }

    public override bool IsValid()
    {
        return RequestId > 0;
    }
}