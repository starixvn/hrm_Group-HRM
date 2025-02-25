namespace HRM.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum VerifiedStatus
    {
        [StringValue("Pending")]
        Pending = 1,

        [StringValue("Verified")]
        Verified = 2,

        [StringValue("NotVerified")]
        NotVerified = 3
    }
}