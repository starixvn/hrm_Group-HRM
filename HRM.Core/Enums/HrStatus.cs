namespace HRM.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum HRStatus
    {
        [StringValue("Pending")]
        Pending = 1,

        [StringValue("Processed")]
        Processed = 2,

        [StringValue("Void")]
        Void = 3
    }
}