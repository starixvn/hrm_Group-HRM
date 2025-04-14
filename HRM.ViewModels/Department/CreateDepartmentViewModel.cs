namespace HRM.ViewModels.Department;

using HRM.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class CreateDepartmentViewModel
{
    private const int MaxSizeName = 200;

    [Required]
    public string Code { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public int UnitId { get; set; }

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Labels.DepartmentId), ResourceType = typeof(Labels))]
    public int GroupDeptId { get; set; }

    [Display(Name = nameof(Labels.PersonalInCharge), ResourceType = typeof(Labels))]
    public int? PICEmpId { get; set; }

    [StringLength(MaxSizeName, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    [Display(Name = nameof(Labels.AccountableArea), ResourceType = typeof(Labels))]
    public string AccountableArea { get; set; }

    public bool IsActive { get; set; } = true;

    public IEnumerable<Models.Unit.Unit> Units { get; set; }
}