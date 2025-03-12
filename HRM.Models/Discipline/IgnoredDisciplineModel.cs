namespace HRM.Models.Discipline;

using System;
using Core.XMLExtensions;

[Serializable]
[XMLName("Row")]
public class IgnoredDisciplineModel
{
    public DateTime WorkDate { get; set; }
    public int EmployeeId { get; set; }
    public short DisciplineTypeId { get; set; }
    public bool IsIgnored { get; set; }
    public bool IsIgnoredChangeShift { get; set; }
    public bool IsIgnoredMissingCheckInOut { get; set; }
    public bool IsIgnoredLateIn { get; set; }
    public bool IsIgnoredEarlyOut { get; set; }
    public bool IsIgnoredAbsent { get; set; }
    public bool IsIgnoredUnplannedLeave { get; set; }
    public bool IsIgnoredUnpaidLeave { get; set; }
    public bool IsIgnoredWorkingUnderTime { get; set; }
    public bool IsIgnoredUnpaidLeaveAbsent { get; set; }
    public bool IsIgnoredWeekendLeave { get; set; }
    public bool IsIgnoredSevenDaysLeave { get; set; }
    public decimal ApproverSubtractScore { get; set; }
    public bool IsOutOfControlReason { get; set; }
}