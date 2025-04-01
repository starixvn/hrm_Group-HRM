namespace HRM.Core.Enums;

using System.ComponentModel;

public enum RequestType
{
    [Description("Check In")]
    CheckIn = 1,

    [Description("Check Out")]
    CheckOut = 2,

    [Description("Late In")]
    LateIn = 3,

    [Description("Early Out")]
    EarlyOut = 4,

    [Description("Change Shift")]
    ChangeShift = 5,

    [Description("Leave")]
    Leave = 6,

    [Description("Compensation Leave")]
    KeepInView = 7,

    [Description("Overtime")]
    Overtime = 8
}