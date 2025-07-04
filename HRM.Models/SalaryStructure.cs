namespace HRM.Models;

public class SalaryStructure : EntityBase<int>
{
    public string LocalSalary { get; set; }

    public string ExpatSalary { get; set; }

    public JobLevel JobLevel { get; set; }
}