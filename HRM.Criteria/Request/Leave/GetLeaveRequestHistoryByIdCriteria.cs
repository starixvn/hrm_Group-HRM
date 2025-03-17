namespace HRM.Criteria.Request.Leave;

using Fanex.Data.Repository;

public class GetLeaveRequestHistoryByIdCriteria : CriteriaBase
{
    public long RequestId { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey() => "LeaveRequest_GetHistoryById";

    public override bool IsValid() => RequestId > 0;
}