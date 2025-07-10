namespace HRM.Models;

public class SalaryStructure
{
    public int SalaryStructureId { get; set; }

    public string Name { get; set; }

    public int JobLevelId { get; set; }

    public string JobLevelName { get; set; }

    public string LocalSalary { get; set; }

    public string ExpatSalary { get; set; }
}