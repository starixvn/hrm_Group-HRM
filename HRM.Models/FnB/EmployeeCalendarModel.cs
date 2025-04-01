using System;

namespace HRM.Models.FnB;

public class EmployeeCalendarModel
{
    public int UserId { get; set; }

    public DateTime WorkDate { get; set; }

    public int ShiftId { get; set; }

    public bool IsHoliday { get; set; }

    public bool IsOffDay { get; set; }

    public bool IsLeaveDay { get; set; }

    public int LeaveTypeId { get; set; }

    public decimal? LeaveDay { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool HasCheckIn { get; set; }
}