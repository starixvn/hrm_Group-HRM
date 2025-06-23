namespace HRM.Models.Report.HeadcountReport;

public class ReportModel
{
    public int? Month { get; set; }
    public int? NewHire { get; set; }
    public int? Replacement { get; set; }
    public int? ProbationResign { get; set; }
    public int? ConfirmedResign { get; set; }
    public int? Transfer { get; set; }
    public int? CurrentHeadCount { get; set; }
    public int? AdditionalHeadCount { get; set; }
    public int? ForecastHeadCount { get; set; }
}