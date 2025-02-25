namespace HRM.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum LateInEarlyOutReason
    {
        [StringValue("Management Allow")]
        ManagementAllow = 1,

        [StringValue("Other")]
        Other
    }
}