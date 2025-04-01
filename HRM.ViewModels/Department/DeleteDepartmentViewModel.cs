using System.ComponentModel.DataAnnotations;

namespace HRM.ViewModels.Department;

public class DeleteDepartmentViewModel
{
    [Required]
    public int Id { get; set; }
}