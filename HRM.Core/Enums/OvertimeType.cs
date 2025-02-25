namespace HRM.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum OvertimeType
    {
        [StringValue("All")]
        All = 0,

        [StringValue("Normal")]
        Normal = 1,

        [StringValue("Off")]
        Off = 2,

        [StringValue("Holiday")]
        Holiday = 3,

        [StringValue("Special")]
        Special = 4,
    }
}