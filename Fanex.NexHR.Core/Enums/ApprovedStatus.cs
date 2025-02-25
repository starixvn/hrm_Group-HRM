namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum ApprovedStatus
    {
        [StringValue("Pending")]
        Pending = 1,

        [StringValue("Approved")]
        Approved = 2,

        [StringValue("Rejected")]
        Rejected = 3
    }
}