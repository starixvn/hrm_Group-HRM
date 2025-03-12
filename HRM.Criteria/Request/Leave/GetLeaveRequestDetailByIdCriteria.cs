namespace HRM.Criteria.Request.Leave;

using Fanex.Data.Repository;

public class GetLeaveRequestDetailByIdCriteria : CriteriaBase
{
    public long RequestId { get; set; }

    public override string GetSettingKey() => "LeaveRequest_GetDetailById";

    public override bool IsValid() => RequestId > 0;
}