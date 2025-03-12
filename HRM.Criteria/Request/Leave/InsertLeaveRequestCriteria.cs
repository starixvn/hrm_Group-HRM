namespace HRM.Criteria.Request.Leave;

public class InsertLeaveRequestCriteria : BaseLeaveRequestCriteria
{
    public int CreatedBy { get; set; }

    public override string GetSettingKey()
    {
        return "LeaveRequest_Insert";
    }

    public override bool IsValid()
    {
        return EmployeeId > 0 && FromDate <= ToDate;
    }
}