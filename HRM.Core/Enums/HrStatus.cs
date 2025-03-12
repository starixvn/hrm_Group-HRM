namespace HRM.Core.Enums;

using System.ComponentModel;

public enum HRStatus
{
    [Description("Pending")]
    Pending = 1,

    [Description("Processed")]
    Processed = 2,

    [Description("Void")]
    Void = 3
}