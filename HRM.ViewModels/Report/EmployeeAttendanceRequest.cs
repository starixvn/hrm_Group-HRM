namespace HRM.ViewModels
{
    using System;
    using HRM.Core;
    using HRM.Models;
    using HRM.Models.Request;

    public class EmployeeAttendanceRequest
    {
        public int EmpId { get; set; }

        public DateTime WorkDate { get; set; }

        public RequestTypeModel RequestType { get; set; }

        public string Remark { get; set; }

        public DateTime RequestDate { get; set; }

        // Leave
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedDateTime => CreatedDate.ToShortFormatedDate();

        public decimal? TotalDays { get; set; }

        public LeaveType LeaveType { get; set; }

        public DateTime? FromTime { get; set; }

        public DateTime? ToTime { get; set; }

        // ChangeShift
        public string FromShift { get; set; }

        public string ToShift { get; set; }

        // LateIn-EarlyOut
        public int? TotalMins { get; set; }

        public int CurrentStatus { get; set; }

        // keep in view
        public byte? KeepInViewTypeId { get; set; }
    }
}