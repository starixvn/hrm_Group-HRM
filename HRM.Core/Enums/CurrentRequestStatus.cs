namespace HRM.Core.Enums
{
    using System.ComponentModel;

    public enum CurrentRequestStatus
    {
        [Description("N/A")]
        NA = 0,

        [Description("Pending")]
        Pending,

        [Description("Verified")]
        Verified,

        [Description("Not Verified")]
        NotVerified,

        [Description("Approved")]
        Approved,

        [Description("Rejected")]
        Rejected,

        [Description("Processed")]
        Processed,

        [Description("Void")]
        Void
    }
}