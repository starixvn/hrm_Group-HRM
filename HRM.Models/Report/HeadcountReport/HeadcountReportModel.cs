using System.Collections.Generic;

namespace HRM.Models.Report.HeadcountReport;

public class HeadcountReportModel
{
    public List<UnitReport> ReportByUnit { get; set; }
    public List<UnitReport> ReportByCompany { get; set; }
    public List<UnitReport> ReportByDetails { get; set; }
}

public class UnitReport
{
    public string Unit { get; set; }
    public bool IsUnitCategory { get; set; }
    public string Company { get; set; }
    public string Country { get; set; }
    public string LocationCode { get; set; }
    public IEnumerable<ReportModel> Details { get; set; }
}