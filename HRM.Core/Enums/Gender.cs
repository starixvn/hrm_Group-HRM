namespace HRM.Core.Enums;

using System.ComponentModel.DataAnnotations;
using Resources;

public enum Gender
{
    [Display(Name = nameof(Labels.Female), ResourceType = typeof(Labels))]
    Female,

    [Display(Name = nameof(Labels.Male), ResourceType = typeof(Labels))]
    Male,

    [Display(Name = nameof(Labels.Unknown), ResourceType = typeof(Labels))]
    Unknown
}