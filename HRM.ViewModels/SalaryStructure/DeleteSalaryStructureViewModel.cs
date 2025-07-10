using System.ComponentModel.DataAnnotations;

namespace HRM.ViewModels.SalaryStructure;

public class DeleteSalaryStructureViewModel
{
    [Required]
    public int Id { get; set; }
}