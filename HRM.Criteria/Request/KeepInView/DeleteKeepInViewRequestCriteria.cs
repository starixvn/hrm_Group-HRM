namespace HRM.Criteria.Request.KeepInView;

using Fanex.Data.Repository;

public class DeleteKeepInViewRequestCriteria : CriteriaBase
{
    public long RequestId { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey() => "KeepInViewRequest_Delete";

    public override bool IsValid() => RequestId > 0;
}