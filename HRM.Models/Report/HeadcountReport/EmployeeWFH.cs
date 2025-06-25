using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Models.Report.HeadcountReport;

public class EmployeeWFH
{
    public int EmpId { get; set; }

    public DateTimeOffset FromDate { get; set; }

    public DateTimeOffset ToDate { get; set; }
}