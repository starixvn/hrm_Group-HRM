namespace HRM.ViewModels.EmployeeGroup;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models;
using Models.Group;
using Resources;

#pragma warning disable S109 // Magic numbers should not be used
public class TeamViewModel
{
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
    [Display(Name = nameof(Labels.Section), ResourceType = typeof(Labels))]
    public int? SectionId { get; set; }

    [EmailAddress(ErrorMessage = "{0} is invalid.")]
    [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    [Display(Name = nameof(Labels.Email), ResourceType = typeof(Labels))]
    public string MailGroup { get; set; }

    public IEnumerable<TeamManager> Managers { get; set; } = new List<TeamManager>();

    [Required(ErrorMessageResourceName = nameof(Messages.RequiredErrorMessage), ErrorMessageResourceType = typeof(Messages))]
    [Display(Name = nameof(Labels.UnitHead), ResourceType = typeof(Labels))]
    public int? HeadId { get; set; }

    [Display(Name = nameof(Labels.UnitHead), ResourceType = typeof(Labels))]
    public string UnitHead { get; set; }

    [Display(Name = nameof(Labels.Manager), ResourceType = typeof(Labels))]
    public string ManagerIds { get; set; }

    [Display(Name = nameof(Labels.Supervisor), ResourceType = typeof(Labels))]
    public string SupervisorIds { get; set; }

    [Display(Name = nameof(Labels.TeamLeader), ResourceType = typeof(Labels))]
    public string LeaderIds { get; set; }

    public IList<Section> Sections { get; set; }

    public IList<Employee> Employees { get; set; }

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
    [Range(0, 5, ErrorMessage = "Number of Offday must be between 0 and 5.")]
    public byte? ExpatOffDaysbyWeek { get; set; }

    [Display(Name = nameof(Labels.WorkingHoursPerWeek), ResourceType = typeof(Labels))]
    [Range(1, 120, ErrorMessage = "Working hours per week must be between 1 and 120.")]
    public byte? ExpatWorkingHoursbyWeek { get; set; }
}