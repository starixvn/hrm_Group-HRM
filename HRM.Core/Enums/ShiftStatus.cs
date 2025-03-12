namespace HRM.Core.Enums;

using System.ComponentModel;

public enum ShiftStatus
{
    [Description("All")]
    All = 0,

    [Description("Active")]
    Active = 1,

    [Description("Disabled")]
    Disabled = 2
}