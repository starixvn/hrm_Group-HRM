namespace Fanex.NexHR.Criteria.Request.Leave
{
    public class UpdateLeaveRequestCriteria : BaseLeaveRequestCriteria
    {
        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveRequest_Update";
        }

        public override bool IsValid()
        {
            return RequestId > 0 && EmployeeId > 0 && FromDate <= ToDate;
        }
    }
}
