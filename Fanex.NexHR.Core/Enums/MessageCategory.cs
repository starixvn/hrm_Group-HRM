namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum MessageCategory
    {
        [StringValue("Notification")]
        Notification = 0,

        [StringValue("Warning")]
        Warning = 1,

        [StringValue("Violation")]
        Violation = 2
    }
}
