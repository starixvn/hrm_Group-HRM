namespace HRM.ViewModels.Employee;

using System.ComponentModel.DataAnnotations;
using HRM.Resources;

public class CreateEmployeeViewModel
{
    private const short FullNameMaxLength = 50;
    private const short NicknameMaxLength = 20;

    [Required]
    [StringLength(FullNameMaxLength, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    public string FullName { get; set; }

    [Required]
    [StringLength(NicknameMaxLength, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    public string Nickname { get; set; }

    [Required]
    [StringLength(FullNameMaxLength, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    public string Username { get; set; }

    [Required]
    [StringLength(NicknameMaxLength, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.StringLengthErrorMessage))]
    public string Code { get; set; }

    [Required]
    public int TeamId { get; set; }
}