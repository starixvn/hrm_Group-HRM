namespace HRM.ViewModels.AnnualLeave;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Constants;
using Resources;

public class BaseAnnualLeaveBalanceViewModel : IValidatableObject
{
    [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage))]
    [Display(ResourceType = typeof(Labels), Name = nameof(Labels.TotalDays))]
    public decimal? TotalDays { get; set; }

    [StringLength(4000, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    [Display(ResourceType = typeof(Labels), Name = nameof(Labels.Description))]
    public string Description { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
    [Display(ResourceType = typeof(Labels), Name = nameof(Labels.AffectFrom))]
    public DateTime AffectedFrom { get; set; }

    public decimal DeductDays => TotalDays.HasValue && TotalDays.Value < 0 ? -TotalDays.Value : 0;

    public decimal CreditDays => TotalDays.HasValue && TotalDays.Value > 0 ? TotalDays.Value : 0;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var result = new List<ValidationResult>();

        if (TotalDays == 0 || TotalDays % Constants.HalfDayOffValue != 0)
        {
            var validation = new ValidationResult($"{Labels.TotalDays} is invalid.", new[] { "TotalDays" });
            result.Add(validation);
        }

        return result;
    }
}