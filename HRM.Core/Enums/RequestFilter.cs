namespace HRM.Core.Enums;

using System.ComponentModel;

public enum RequestFilter
{
    [Description("All")]
    All = 0,

    [Description("Check In/Check Out")]
    CheckInOut = 1,

    [Description("Late In/Early Out")]
    LateInEarlyOut = 2,

    [Description("Change Shift")]
    ChangeShift = 3,

    [Description("Leave")]
    Leave = 4,

    [Description("Compensation Leave")]
    KeepInView = 5
}