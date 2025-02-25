namespace HRM.Criteria.Request.Leave
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetEmployeeCalendarCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "LeaveRequest_GetCalendar";

        public override bool IsValid() => EmployeeId > 0 && FromDate <= ToDate;
    }
}