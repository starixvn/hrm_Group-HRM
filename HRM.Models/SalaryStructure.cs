namespace HRM.Models;

public class SalaryStructure
{
    public int SalaryStructureId { get; set; }
    public string Name { get; set; }
    public string LocalSalary { get; set; }

    public string ExpatSalary { get; set; }

    public JobLevel JobLevel { get; set; }
}