using HRM.Models.Report.HeadcountReport;

namespace HRM.Services.Interfaces.Report.Headcount;

public interface IHeadcountExportService
{
    byte[] Export(string templatePath, HeadcountReportModel headcountData, int selectedYear);
}