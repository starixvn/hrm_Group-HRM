namespace HRM.ViewModels.Department;

using System.ComponentModel.DataAnnotations;

public class EditDepartmentViewModel : CreateSalaryStructureViewModel
{
    [Required]
    public int SalaryStructureId { get; set; }
}