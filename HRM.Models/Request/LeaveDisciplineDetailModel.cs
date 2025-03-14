namespace HRM.Models.Request;

using System;

public class LeaveDisciplineDetailModel
{
    public DateTime WorkDate { get; set; }

    public int LeaveTypeId { get; set; }

    public string LeaveTypeName { get; set; }

    public bool IsHalfDay { get; set; }

    public DateTime FromTime { get; set; }

    public DateTime ToTime { get; set; }

    public bool IsFromAfterday { get; set; }

    public bool IsToAfterday { get; set; }
}