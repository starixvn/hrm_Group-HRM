namespace HRM.ViewModels.SalaryStructure;

using System.ComponentModel.DataAnnotations;

public class EditSalaryStructureViewModel : CreateSalaryStructureViewModel
{
    [Required]
    public int SalaryStructureId { get; set; }
}