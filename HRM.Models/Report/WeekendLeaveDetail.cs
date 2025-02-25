namespace HRM.Models.Report
{
    using System;

    public class WeekendLeaveDetail
    {
        public int EmployeeId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public bool IsLocal { get; set; }

        public string TeamId { get; set; }

        public string TeamCode { get; set; }

        public string TeamName { get; set; }

        public DateTime LeaveDate { get; set; }

        public float LeaveDay { get; set; }

        public string LeaveType { get; set; }

        public int LeaveTypeId { get; set; }

        public string Remark { get; set; }

        public string ApproverComment { get; set; }
    }
}