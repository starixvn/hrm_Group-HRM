namespace HRM.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum ShiftStatus
    {
        [StringValue("All")]
        All = 0,

        [StringValue("Active")]
        Active = 1,

        [StringValue("Disabled")]
        Disabled = 2
    }
}