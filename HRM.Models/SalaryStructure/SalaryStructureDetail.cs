using System;

namespace HRM.Models.SalaryStructure;

[Serializable]
public class SalaryStructureDetail
{
    public int SalaryStructureId { get; set; }

    public int JobLevelId { get; set; }

    public string Name { get; set; }

    public string BasicSalary { get; set; }
    public string ExpatSalary { get; set; }
}