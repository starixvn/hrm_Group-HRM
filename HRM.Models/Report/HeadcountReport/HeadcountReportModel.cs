using System.Collections.Generic;

namespace HRM.Models.Report.HeadcountReport;

public class HeadcountReportModel
{
    public List<UnitReport> ReportByUnit { get; set; }
    public List<UnitReport> ReportByCompany { get; set; }
    public List<UnitReport> ReportByDetails { get; set; }
    public List<UnitReport> ReportByUnitCountry { get; set; }
    public List<UnitReport> ReportByUnitCountryLocal { get; set; }
    public List<UnitReport> ReportByUnitCountryExpat { get; set; }
}

public class UnitReport
{
    public string Label1 { get; set; }
    public string Label2 { get; set; }
    public string Label3 { get; set; }
    public bool IsUnitCategory { get; set; }
    public bool IsGrandTotal { get; set; }
    public int BUCount { get; set; }
    public IEnumerable<ReportModel> Details { get; set; } = new List<ReportModel>();
}