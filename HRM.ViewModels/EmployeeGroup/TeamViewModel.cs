namespace HRM.ViewModels.EmployeeGroup;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models;
using Resources;

#pragma warning disable S109 // Magic numbers should not be used
public class TeamViewModel
{
    private const int MaxSizeName = 200;

    public int Id { get; set; }

    [StringLength(20, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    [Display(Name = nameof(Labels.Code), ResourceType = typeof(Labels))]
    public string TeamCode { get; set; }

    [StringLength(150, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    [Display(Name = nameof(Labels.Name), ResourceType = typeof(Labels))]
    public string TeamName { get; set; }

    [Display(Name = nameof(Labels.Department), ResourceType = typeof(Labels))]
    public int? DepartmentId { get; set; }

    [EmailAddress(ErrorMessage = "{0} is invalid.")]
    [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    [Display(Name = nameof(Labels.Email), ResourceType = typeof(Labels))]
    public string MailGroup { get; set; }

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

    public IList<Employee> Employees { get; set; }

    public List<EmployeePIC> EmployeePics { get; set; }

    public int CompanyId { get; set; }

    public string DepartmentName { get; set; }

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Labels.BasicAnnualLeaveBalance), ResourceType = typeof(Labels))]
    [Range(0, int.MaxValue, ErrorMessage = "Basic Annual Leave Balance must be greater than 0.")]
    public byte? LocalBasicAnnualLeaveBalance { get; set; }

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Labels.WorkingDaysPerMonth), ResourceType = typeof(Labels))]
    [Range(1, 31, ErrorMessage = "Working days per Month must be between 1 and 31.")]
    public byte? LocalWorkingDays { get; set; }

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Labels.WorkingHoursPerDay), ResourceType = typeof(Labels))]
    [Range(1, 24, ErrorMessage = "Working hours per day must be between 1 and 24.")]
    public byte? LocalWorkingHours { get; set; }

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Labels.NumberOfOffday), ResourceType = typeof(Labels))]
    [Range(0, 5, ErrorMessage = "Number of Offday must be between 0 and 5.")]
    public byte? LocalOffDaysbyWeek { get; set; }

    [Display(Name = nameof(Labels.WorkingHoursPerWeek), ResourceType = typeof(Labels))]
    [Range(1, 120, ErrorMessage = "Working hours per week must be between 1 and 120.")]
    public byte? LocalWorkingHoursbyWeek { get; set; }

    public byte? LocalYearsOfServiceForExtraLeave { get; set; }

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Labels.BasicAnnualLeaveBalance), ResourceType = typeof(Labels))]
    [Range(0, int.MaxValue, ErrorMessage = "Basic Annual Leave Balance must be greater than 0.")]
    public byte? ExpatBasicAnnualLeaveBalance { get; set; }

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Labels.WorkingDaysPerMonth), ResourceType = typeof(Labels))]
    [Range(1, 31, ErrorMessage = "Working days per Month must be between 1 and 31.")]
    public byte? ExpatWorkingDays { get; set; }

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Labels.WorkingHoursPerDay), ResourceType = typeof(Labels))]
    [Range(1, 24, ErrorMessage = "Working hours per day must be between 1 and 24.")]
    public byte? ExpatWorkingHours { get; set; }

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Labels.NumberOfOffday), ResourceType = typeof(Labels))]
    [Range(0, 5, ErrorMessage = "Number of Off day must be between 0 and 5.")]
    public byte? ExpatOffDaysbyWeek { get; set; }

    [Display(Name = nameof(Labels.WorkingHoursPerWeek), ResourceType = typeof(Labels))]
    [Range(1, 120, ErrorMessage = "Working hours per week must be between 1 and 120.")]
    public byte? ExpatWorkingHoursbyWeek { get; set; }

    public byte? ExpatYearsOfServiceForExtraLeave { get; set; }
}