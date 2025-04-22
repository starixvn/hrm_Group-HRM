namespace HRM.ViewModels.EmployeeGroup;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models;
using Resources;

#pragma warning disable S109 // Magic numbers should not be used
public class GroupTeamViewModel
{
    private const int MaxSizeName = 200;

    public int Id { get; set; }

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [StringLength(20, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    [Display(Name = nameof(Labels.Code), ResourceType = typeof(Labels))]
    public string TeamCode { get; set; }

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [StringLength(150, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    [Display(Name = nameof(Labels.Name), ResourceType = typeof(Labels))]
    public string TeamName { get; set; }

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Labels.Department), ResourceType = typeof(Labels))]
    public int? DepartmentId { get; set; }

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Labels.TeamId), ResourceType = typeof(Labels))]
    public int GroupTeamId { get; set; }

    [Display(Name = nameof(Labels.PersonalInCharge), ResourceType = typeof(Labels))]
    public int? PICEmpId { get; set; }

    [Display(Name = nameof(Labels.Company), ResourceType = typeof(Labels))]
    public List<Company> Companies { get; set; }

    [StringLength(MaxSizeName, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    [Display(Name = nameof(Labels.AccountableArea), ResourceType = typeof(Labels))]
    public string AccountableArea { get; set; }

    public IList<Department> Departments { get; set; }

    public List<EmployeePIC> EmployeePics { get; set; }

    public int CompanyId { get; set; }
}