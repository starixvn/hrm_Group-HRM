namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum RequestFilter
    {
        [StringValue("All")]
        All = 0,

        [StringValue("Check In/Check Out")]
        CheckInOut = 1,

        [StringValue("Late In/Early Out")]
        LateInEarlyOut = 2,

        [StringValue("Change Shift")]
        ChangeShift = 3,

        [StringValue("Leave")]
        Leave = 4,

        [StringValue("Compensation Leave")]
        KeepInView = 5
    }
}