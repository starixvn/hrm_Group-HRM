using System.Collections.Generic;

namespace HRM.ViewModels.SalaryStructure;

public class BasicSalaryConfigOption
{
    public string Value { get; set; }
    public string Label { get; set; }
}

public class BasicSalaryConfigViewModel
{
    public string Config { get; set; }
    public IEnumerable<BasicSalaryConfigOption> ConfigOptions { get; set; }
}


