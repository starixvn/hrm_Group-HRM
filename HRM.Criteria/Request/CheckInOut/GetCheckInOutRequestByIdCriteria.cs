namespace HRM.Criteria.Request.CheckInOut;

using Fanex.Data.Repository;

public class GetCheckInOutRequestByIdCriteria : CriteriaBase
{
    public long RequestId { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey() => "CheckInOutRequest_GetById";

    public override bool IsValid() => RequestId > 0;
}