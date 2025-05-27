namespace HRM.ViewModels.CompanySecondmentEmployee;

using System.ComponentModel.DataAnnotations;

public class DeleteCompanySecondEmployeeViewModel
{
    [Required]
    public int EmpId { get; set; }
}