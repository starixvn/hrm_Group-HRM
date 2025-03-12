namespace HRM.Core.Enums;

using System.ComponentModel;

public enum MarriageStatus
{
    [Description("Unknown")]
    Default = 0,

    [Description("Married")]
    Married = 1,

    [Description("Divorced")]
    Divorced = 2,

    [Description("Single")]
    Single = 3,

    [Description("Widow")]
    Widow = 4
}