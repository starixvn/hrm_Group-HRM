namespace HRM.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum WorkType
    {
        [StringValue("Off")]
        Off = 1,

        [StringValue("Leave")]
        Leave = 2,

        [StringValue("Work")]
        Work = 3
    }
}