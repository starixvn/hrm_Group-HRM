namespace HRM.Criteria.Request.LateInEarlyOut;

using Fanex.Data.Repository;

public class GetLateInEarlyOutRequestByIdCriteria : CriteriaBase
{
    public short CompanyId { get; set; }
    public long RequestId { get; set; }

    public override string GetSettingKey() => "LateInEarlyOutRequest_GetById";

    public override bool IsValid() => RequestId > 0;
}