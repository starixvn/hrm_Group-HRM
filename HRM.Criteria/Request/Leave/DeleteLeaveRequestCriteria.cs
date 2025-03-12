namespace HRM.Criteria.Request.Leave;

using Fanex.Data.Repository;

public class DeleteLeaveRequestCriteria : CriteriaBase
{
    public long RequestId { get; set; }

    public override string GetSettingKey()
    {
        return "LeaveRequest_Delete";
    }

    public override bool IsValid()
    {
        return RequestId > 0;
    }
}