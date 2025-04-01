namespace HRM.Criteria.Request.Leave;

using Fanex.Data.Repository;

public class GetLeaveRequestByIdCriteria : CriteriaBase
{
    public long RequestId { get; set; }

    public short CompanyId { get; set; }

    public override string GetSettingKey() => "LeaveRequest_GetById";

    public override bool IsValid() => RequestId > 0;
}