using System.Threading.Tasks;
using HRM.Models.Report.HeadcountReport;
using HRM.Repositories.HeadcountReport;
using HRM.Services.Interfaces.Report.Headcount;

namespace HRM.Services.Implement.Report.Headcount;

public class CalculateHeadcountService(IHeadcountReportRepository reportRepository) : IHeadcountReportService
{
    public async Task<HeadcountReportModel> GetReportAsync(int selectedYear)
    {
        var data = await reportRepository.GetHeadcountDataAsync(selectedYear);
        
        var unitCalculator = new UnitReportCalculator(data);
        var companyCalculator = new CompanyReportCalculator(data);
        var detailsCalculator = new DetailsReportCalculator(data);
        var unitCountryCalculator = new UnitCountryReportCalculator(data);

        var reportTasks = new[]
        {
            Task.Run(() => unitCalculator.Calculate()),
            Task.Run(() => companyCalculator.Calculate()),
            Task.Run(() => detailsCalculator.Calculate()),
            Task.Run(() => unitCountryCalculator.Calculate(null)),
            Task.Run(() => unitCountryCalculator.Calculate(true)),
            Task.Run(() => unitCountryCalculator.Calculate(false))
        };

        var results = await Task.WhenAll(reportTasks);
        return new HeadcountReportModel
        {
            ReportByUnit = results[0],
            ReportByCompany = results[1],
            ReportByDetails = results[2],
            ReportByUnitCountry = results[3],
            ReportByUnitCountryLocal = results[4],
            ReportByUnitCountryExpat = results[5]
        };
    }
}