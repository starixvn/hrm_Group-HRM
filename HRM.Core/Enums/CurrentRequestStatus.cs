namespace HRM.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum CurrentRequestStatus
    {
        [StringValue("N/A")]
        NA = 0,

        [StringValue("Pending")]
        Pending,

        [StringValue("Verified")]
        Verified,

        [StringValue("Not Verified")]
        NotVerified,

        [StringValue("Approved")]
        Approved,

        [StringValue("Rejected")]
        Rejected,

        [StringValue("Processed")]
        Processed,

        [StringValue("Void")]
        Void
    }
}