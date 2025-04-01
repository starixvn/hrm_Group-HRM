namespace HRM.Core.Enums;

using System.ComponentModel;

public enum CheckInOutReason
{
    [Description("Van Late")]
    VanLate = 1,

    [Description("Business Trip")]
    BusinessTrip = 2,

    [Description("Forgot")]
    Forgot = 3,

    [Description("Other")]
    Other = 4
}