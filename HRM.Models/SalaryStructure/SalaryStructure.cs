using System.Collections.Generic;

namespace HRM.Models.SalaryStructure;

public class SalaryStructure
{
    public int SalaryStructureId { get; set; }
    public string Name { get; set; }

    public IEnumerable<SalaryStructureDetail> Details { get; set; }
}