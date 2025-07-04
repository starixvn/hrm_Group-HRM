namespace HRM.ViewModels.SalaryStructure;

using HRM.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class CreateSalaryStructureViewModel
{
    [Required]
    public string SalaryStructureName { get; set; }

    public int? LocalSalary { get; set; }

    public int? ExpatSalary { get; set; }

    [Required]
    public IEnumerable<Models.JobLevel.JobLevel> JobLevels { get; set; }
}