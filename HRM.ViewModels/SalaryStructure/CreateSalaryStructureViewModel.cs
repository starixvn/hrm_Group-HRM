namespace HRM.ViewModels.SalaryStructure;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class CreateSalaryStructureViewModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    public int JobLevelId { get; set; }

    public string LocalSalary { get; set; }

    public string ExpatSalary { get; set; }

    [Required]
    public IEnumerable<Models.JobLevel> JobLevels { get; set; }
}