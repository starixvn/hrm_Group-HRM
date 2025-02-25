namespace HRM.Criteria.Request.Leave
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetTotalLeaveDayCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long? RequestId { get; set; }
        public int LeaveTypeId { get; set; }

        public override string GetSettingKey() => "LeaveRequest_GetTotalLeaveDay";

        public override bool IsValid() => EmployeeId > 0 && FromDate <= ToDate;
    }
}