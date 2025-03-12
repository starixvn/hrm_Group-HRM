using System;

namespace HRM.Models.Request.Leave;

public class LeaveRequestDetail
{
    public long HistoryId { get; set; }
    public long RequestId { get; set; }

    public DateTime OnDate { get; set; }

    public string ScheduleStatus { get; set; }

    public int WorkType { get; set; }

    public int LeaveTypeId { get; set; }

    public string LeaveTypeName { get; set; }

    public bool IsHalfDay { get; set; }

    public DateTime FromTime { get; set; }

    public DateTime ToTime { get; set; }

    public bool IsOffOrHoliday { get; set; }

    public bool IsFromAfterday { get; set; }

    public bool IsToAfterday { get; set; }

    public bool IsUnpaidLeaveAbsent { get; set; }

    public bool IsAbsent { get; set; }

    public bool IsUnplannedLeave { get; set; }
}