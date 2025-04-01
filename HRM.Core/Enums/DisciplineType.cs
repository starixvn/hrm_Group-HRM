namespace HRM.Core.Enums;

using System.ComponentModel;

public enum DisciplineType
{
    [Description("Change Shift")]
    ChangeShift = 1,

    [Description("Missing Check In/Out")]
    MissingCheckInOut = 2,

    [Description("Late In")]
    LateIn = 3,

    [Description("Early Out")]
    EarlyOut = 4,

    [Description("Absent")]
    Absent = 5,

    [Description("Unpaid Leave")]
    UnpaidLeave = 6,

    [Description("Working Under Time")]
    WorkingUnderTime = 7,

    [Description("Unpaid Leave Absent")]
    UnpaidLeaveAbsent = 8,

    [Description("Unplanned Leave")]
    UnplannedLeave = 9,

    [Description("Weekend Leave")]
    WeekendLeave = 10,

    [Description("Seven Days Leave")]
    SevenDaysLeave = 11
}