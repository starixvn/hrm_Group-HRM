namespace HRM.Models.HospitalizationLeave;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HRM.Core.Constants;
using HRM.Resources;

public class HospitalizationLeaveTransaction : IValidatableObject
{
    private const short DescriptionMaxLength = 4000;
    public long TransId { get; set; }
    public int EmpId { get; set; }
    public string Nickname { get; set; }
    public long? RequestId { get; set; }
    public DateTime ModifiedDate { get; set; }

    [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage))]
    [Display(ResourceType = typeof(Labels), Name = nameof(Labels.TotalDays))]
    public decimal? TotalDays { get; set; }

    [StringLength(DescriptionMaxLength, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
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
        IList<ValidationResult> result = new List<ValidationResult>();

        if (TotalDays == 0 || TotalDays % Constants.HalfDayOffValue != 0)
        {
            var validation = new ValidationResult($"{Labels.TotalDays} is invalid.", new[] { "TotalDays" });
            result.Add(validation);
        }

        return result;
    }
}