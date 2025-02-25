namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum KeepInViewTypes
    {
        [StringValue("Compensation Leave By Day")]
        KeepInViewByDay = 1,

        [StringValue("Compensation Leave By Hour")]
        KeepInViewByHour = 2
    }
}