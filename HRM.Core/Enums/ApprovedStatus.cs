namespace HRM.Core.Enums
{
    using System.ComponentModel;

    public enum ApprovedStatus
    {
        [Description("Pending")]
        Pending = 1,

        [Description("Approved")]
        Approved = 2,

        [Description("Rejected")]
        Rejected = 3
    }
}