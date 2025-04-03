namespace HRM.Models.Report.MonthlyWorkingHours;

using System;

public class EmployeeRequest
{
    public long RequestId { get; set; }
    public short RequestTypeId { get; set; }
    public byte RequestStatusId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime RequestedDate { get; set; }
    public string Remark { get; set; }

    public int EmployeeId { get; set; }
    public string Code { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }
    public string Team { get; set; }
    public string Department { get; set; }

    // leave
    public ushort LeaveTypeId { get; set; }
    public string LeaveType { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public bool IsHalfDay { get; set; }
    public decimal TotalDays { get; set; }
    public string LeaveCountDetails { get; set; }

    // Change shift
    public string FromShift { get; set; }
    public string ToShift { get; set; }

    // Late In - Early Out
    public int TotalMinutes { get; set; }
    public int ActualInOutMins { get; set; }

    // Keep In View
    public DateTime FromTime { get; set; }
    public DateTime ToTime { get; set; }
    public byte KeepInViewTypeId { get; set; }
}