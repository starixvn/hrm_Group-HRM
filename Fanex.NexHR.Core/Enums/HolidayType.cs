namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Resources;

    public enum HolidayType
    {
        [Display(Name = nameof(Labels.Regular), ResourceType = typeof(Labels))]
        Regular = 1,

        [Display(Name = nameof(Labels.Special), ResourceType = typeof(Labels))]
        Special
    }
}