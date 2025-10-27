using System;
using System.Collections.Generic;
using System.Linq;

namespace HRM.Models.Report.HeadcountReport;

public class HeadcountData
{
    public int Year { get; set; }
    public List<EmployeeInfo> EmployeeInfos { get; set; } = [];
    public List<EmployeeWFH> AllEmployeeWFHs { get; set; } = [];
    public HashSet<int> WfhEmployeeIds => [..AllEmployeeWFHs.Select(wfh => wfh.EmpId)];
    public List<EmployeeTransferData> EmployeeTransferIns { get; set; } = [];
    public List<EmployeeTransferData> EmployeeTransferOut { get; set; } = [];
}

public class EmployeeTransferData
{
    public int TransferId { get; set; }
    public int EmpId { get; set; }
    public int? FromCompanyId { get; set; }
    public int? ToCompanyId { get; set; }
    public DateTime TransferDate { get; set; }
}