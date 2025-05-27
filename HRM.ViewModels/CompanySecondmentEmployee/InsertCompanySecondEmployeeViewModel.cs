namespace HRM.ViewModels.CompanySecondmentEmployee;

using System.ComponentModel.DataAnnotations;

public class CreateCompanySecondEmployeeViewModel
{
    [Required]
    public int EmpId { get; set; }

    [Required]
    public int CompanyId { get; set; }
}