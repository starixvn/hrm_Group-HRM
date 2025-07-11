using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HRM.Models.SalaryStructure;

namespace HRM.ViewModels.SalaryStructure;

public class CreateSalaryStructureViewModel
{
    [Required]
    public string Name { get; set; }

    public IEnumerable<SalaryStructureDetail> Details { get; set; }
}