using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Models.GroupDashboard;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Interfaces;
using HRM.Services.Interfaces.Report.Headcount;

namespace HRM.Services.Implement;

public class GroupDashboardService(
    IHeadcountReportService headcountReportService,
    IMonthlyPayrollService payrollService) : IGroupDashboardService
{
    public async Task<DashboardViewModel> GetDashboardDataAsync(int year)
    {
        var data = await headcountReportService.GetReportAsync(year);
        var grandTotalUnit = data.ReportByUnit.Find(x => x.IsGrandTotal);

        return new DashboardViewModel
        {
            CurrentHeadcount = GetCurrentHeadcount(grandTotalUnit),
            Recruitment = GetRecruitment(grandTotalUnit),
            ProbationResign = GetProbationResign(grandTotalUnit),
            ConfirmedResign = GetConfirmedResign(grandTotalUnit),
            Companies = GetCompanyCount(data.ReportByCompany),
            AvgAge = 35,
            AvgYrsTenure = 10,
            HeadcountByBU = GetHeadcountByBU(data.ReportByUnit),
            HeadcountByCountry = GetHeadcountByCountry(data.ReportByUnitCountry),
            BuCategory = GetBuCategory(data.ReportByUnit),
            EmployeeByAge = GetEmployeeByAge(),
            SalaryExpenseByBU = GetSalaryExpenseByBU(data.ReportByUnit)
        };
    }

    private static int GetCurrentHeadcount(UnitReport grandTotalUnit) => grandTotalUnit?.Details?.LastOrDefault()?.CurrentHeadCount ?? 0;

    private static int GetRecruitment(UnitReport grandTotalUnit) => grandTotalUnit?.Details?.LastOrDefault()?.NewHire ?? 0;

    private static int GetProbationResign(UnitReport grandTotalUnit) => grandTotalUnit?.Details?.Sum(x => x.ProbationResign) ?? 0;

    private static int GetConfirmedResign(UnitReport grandTotalUnit) => grandTotalUnit?.Details?.Sum(x => x.ConfirmedResign) ?? 0;

    private static int GetCompanyCount(IEnumerable<UnitReport> reportByCompany) => reportByCompany.Count(x => !x.IsGrandTotal && !x.IsUnitCategory);

    private static IEnumerable<HeadcountModel> GetHeadcountByBU(IEnumerable<UnitReport> reportByUnit)
    {
        return reportByUnit
            .Where(x => !x.IsGrandTotal && !x.IsUnitCategory)
            .Select(x => new HeadcountModel
            {
                Label = x.Label1,
                Value = x.Details?.LastOrDefault()?.CurrentHeadCount ?? 0
            });
    }

    private static IEnumerable<HeadcountModel> GetHeadcountByCountry(IEnumerable<UnitReport> reportByUnitCountry)
    {
        return reportByUnitCountry
            .Where(x => x.IsGrandTotal)
            .Select(x => new HeadcountModel
            {
                Label = x.Label3,
                Value = x.Details?.LastOrDefault()?.CurrentHeadCount ?? 0
            });
    }

    private static IEnumerable<CategoryModel> GetBuCategory(IEnumerable<UnitReport> reportByUnit)
    {
        return reportByUnit
            .Where(x => x.IsUnitCategory)
            .GroupBy(x =>
            {
                var text = x.Label1?.Replace(" TOTAL", "") ?? string.Empty;
                var index = text.IndexOf('(');
                return index > 0 ? text[..index].Trim() : text.Trim();
            })
            .Select(g => new CategoryModel
            {
                Category = g.Key,
                EmployeeCount = g.First().Details?.LastOrDefault()?.CurrentHeadCount ?? 0,
                BUCount = g.First().BUCount
            })
            .OrderBy(x => x.Category);
    }

    private IEnumerable<HeadcountModel> GetSalaryExpenseByBU(IEnumerable<UnitReport> reportByUnit)
    {
        return reportByUnit
            .Where(x => !x.IsGrandTotal && !x.IsUnitCategory)
            .Select(_ => new HeadcountModel());
    }

    private static IEnumerable<HeadcountModel> GetEmployeeByAge()
    {
        return new List<HeadcountModel>
        {
            new() { Label = "20-24", Value = 180 },
            new() { Label = "25-29", Value = 320 },
            new() { Label = "30-34", Value = 430 },
            new() { Label = "35-39", Value = 380 },
            new() { Label = "40-44", Value = 280 },
            new() { Label = "45-49", Value = 180 },
            new() { Label = "50-54", Value = 120 },
            new() { Label = "55-59", Value = 80 }
        };
    }
}

