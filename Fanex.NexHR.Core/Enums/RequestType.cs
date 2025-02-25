namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum RequestType
    {
        [StringValue("Check In")]
        CheckIn = 1,

        [StringValue("Check Out")]
        CheckOut = 2,

        [StringValue("Late In")]
        LateIn = 3,

        [StringValue("Early Out")]
        EarlyOut = 4,

        [StringValue("Change Shift")]
        ChangeShift = 5,

        [StringValue("Leave")]
        Leave = 6,

        [StringValue("Compensation Leave")]
        KeepInView = 7,

        [StringValue("Overtime")]
        Overtime = 8
    }
}