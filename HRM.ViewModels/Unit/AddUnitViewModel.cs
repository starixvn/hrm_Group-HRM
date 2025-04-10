namespace HRM.ViewModels.Unit;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Resources;

public class AddUnitViewModel
{
    private const int MaxSizeCode = 50;
    private const int MaxSizeName = 200;

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [StringLength(MaxSizeCode, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    [Display(Name = nameof(Labels.Code), ResourceType = typeof(Labels))]
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Labels.BUId), ResourceType = typeof(Labels))]
    public int GroupUnitId { get; set; }

    [Display(Name = nameof(Labels.PersonalInCharge), ResourceType = typeof(Labels))]
    public int? PICEmpId { get; set; }

    [StringLength(MaxSizeName, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    [Display(Name = nameof(Labels.BUCategory), ResourceType = typeof(Labels))]
    public string Category { get; set; }

    [StringLength(MaxSizeName, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    [Display(Name = nameof(Labels.BUType), ResourceType = typeof(Labels))]
    public string Type { get; set; }

    [StringLength(MaxSizeName, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    [Display(Name = nameof(Labels.AccountableArea), ResourceType = typeof(Labels))]
    public string AccountableArea { get; set; }

    public bool IsActive { get; set; } = true;
}
