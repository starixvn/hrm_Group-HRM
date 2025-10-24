using System.Collections.Generic;
using HRM.Models.Report.HeadcountReport;

namespace HRM.Models.Report.HeadcountReport;

public class HeadcountData
{
    public int Year { get; set; }
    public IEnumerable<EmployeeInfo> EmployeeInfos { get; set; } = [];
    public IEnumerable<EmployeeWFH> AllEmployeeWFHs { get; set; } = [];
    public HashSet<int> WfhEmployeeIds { get; set; } = [];
}
