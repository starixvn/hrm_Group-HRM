using System;

namespace HRM.Models.Report.HeadcountReport;

public class EmployeeWFH
{
    public int EmpId { get; set; }

    public DateTimeOffset FromDate { get; set; }

    public DateTimeOffset ToDate { get; set; }
}