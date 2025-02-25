namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum AttendanceFilter
    {
        [StringValue("All")]
        All = 0,

        [StringValue("MissingCheckIn")]
        MissingCheckIn = 1,

        [StringValue("MissingCheckOut")]
        MissingCheckOut = 2,

        [StringValue("LateIn")]
        LateIn = 3,

        [StringValue("EarlyOut")]
        EarlyOut = 4,

        [StringValue("NotWork")]
        NotWork = 5
    }
}