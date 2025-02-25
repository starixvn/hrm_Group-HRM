namespace HRM.Core.Enums
{
    using System.ComponentModel;

    public enum AttendanceFilter
    {
        [Description("All")]
        All = 0,

        [Description("MissingCheckIn")]
        MissingCheckIn = 1,

        [Description("MissingCheckOut")]
        MissingCheckOut = 2,

        [Description("LateIn")]
        LateIn = 3,

        [Description("EarlyOut")]
        EarlyOut = 4,

        [Description("NotWork")]
        NotWork = 5
    }
}