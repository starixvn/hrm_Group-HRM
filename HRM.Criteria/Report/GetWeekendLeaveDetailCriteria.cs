namespace HRM.Criteria.Report
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetWeekendLeaveDetailCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string TeamId { get; set; }

        public int? EmployeeId { get; set; }

        public bool? IsLocal { get; set; }

        [SpParam(Ignored = true)]
        public string EmployeeName { get; set; }

        [SpParam(Ignored = true)]
        public string TeamName { get; set; }

        public int LeaveTypeId { get; set; }

        public int SupervisorId { get; set; }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }

        public override string GetSettingKey()
        {
            return "Report_GetWeekendLeaveDetail";
        }
    }
}