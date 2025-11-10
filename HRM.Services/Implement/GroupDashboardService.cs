using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Models.Group;
using HRM.Models.GroupDashboard;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Helper;
using HRM.Services.Interfaces;
using HRM.Services.Interfaces.Report.Headcount;

namespace HRM.Services.Implement;

public class GroupDashboardService(
    IHeadcountReportService headcountReportService,
    IMonthlyPayrollService payrollService,
    IEmployeeService employeeService) : IGroupDashboardService
{
    public async Task<DashboardViewModel> GetDashboardDataAsync(int year)
    {
        var data = await headcountReportService.GetReportAsync(year);
        var grandTotalUnit = data.ReportByUnit.Find(x => x.IsGrandTotal);
        var employees = await employeeService.GetEmployeesInGroupAsync();
        var salaryByEmpDict = await GetSalaryExpense(grandTotalUnit);
        var currentHeadcountEmpIds = GetCurrentHeadcountEmpIds(grandTotalUnit);

        var filteredEmployees = employees
            .Where(e => currentHeadcountEmpIds.Contains(e.EmpId));

        var (AverageAge, AgeRanges) = GetEmployeeByAge(filteredEmployees);

        return new DashboardViewModel
        {
            CurrentHeadcount = GetCurrentHeadcount(grandTotalUnit),
            Recruitment = GetRecruitment(grandTotalUnit),
            ProbationResign = GetProbationResign(grandTotalUnit),
            ConfirmedResign = GetConfirmedResign(grandTotalUnit),
            Companies = GetCompanyCount(data.ReportByCompany),
            AvgAge = AverageAge,
            AvgYrsTenure = GetAverageTenure(filteredEmployees),
            HeadcountByBU = GetHeadcountByBU(data.ReportByUnit),
            HeadcountByCountry = GetHeadcountByCountry(data.ReportByUnitCountry),
            BuCategory = GetBuCategory(data.ReportByUnit),
            EmployeeByAge = AgeRanges,
            SalaryExpenseByBU = GetSalaryExpenseByBU(data.ReportByUnit, salaryByEmpDict),
            SalaryExpenseByBUCategory = GetSalaryExpenseByBUCategory(data.ReportByUnit, salaryByEmpDict)
        };
    }

    #region Grand Total Unit Helpers

    private static IEnumerable<int> GetCurrentHeadcountEmpIds(UnitReport grandTotalUnit) => GetLastMonthReport(grandTotalUnit)?.CurrentHeadCountEmpIds ?? [];

    private static int GetCurrentHeadcount(UnitReport grandTotalUnit) => GetLastMonthReport(grandTotalUnit)?.CurrentHeadCount ?? 0;

    private static int GetRecruitment(UnitReport grandTotalUnit) => grandTotalUnit?.Details?.Sum(x => x.NewHire) ?? 0;

    private static int GetProbationResign(UnitReport grandTotalUnit) => grandTotalUnit?.Details?.Sum(x => x.ProbationResign) ?? 0;

    private static int GetConfirmedResign(UnitReport grandTotalUnit) => grandTotalUnit?.Details?.Sum(x => x.ConfirmedResign) ?? 0;

    private static ReportModel GetLastMonthReport(UnitReport unitReport) => unitReport?.Details?.LastOrDefault();

    #endregion

    #region Report Helpers

    private static int GetCompanyCount(IEnumerable<UnitReport> reportByCompany) 
        => reportByCompany.Count(x => !x.IsGrandTotal && !x.IsUnitCategory);

    private static IEnumerable<HeadcountModel> GetHeadcountByBU(IEnumerable<UnitReport> reportByUnit)
    {
        return reportByUnit
            .Where(x => !x.IsGrandTotal && !x.IsUnitCategory)
            .Select(x => new HeadcountModel
            {
                Label = x.Label1,
                Value = GetLastMonthReport(x)?.CurrentHeadCount ?? 0
            });
    }

    private static IEnumerable<HeadcountModel> GetHeadcountByCountry(IEnumerable<UnitReport> reportByUnitCountry)
    {
        return reportByUnitCountry
            .Where(x => x.IsGrandTotal)
            .Select(x => new HeadcountModel
            {
                Label = x.Label3,
                Value = GetLastMonthReport(x)?.CurrentHeadCount ?? 0
            });
    }

    private static IEnumerable<CategoryModel> GetBuCategory(IEnumerable<UnitReport> reportByUnit)
    {
        return reportByUnit
            .Where(x => x.IsUnitCategory)
            .GroupBy(x => HeadcountReportHelper.ExtractCategoryName(x.Label1))
            .Select(g => new CategoryModel
            {
                Category = g.Key,
                EmployeeCount = g.Sum(x => GetLastMonthReport(x)?.CurrentHeadCount ?? 0),
                BUCount = g.Sum(x => x.BUCount)
            })
            .OrderBy(x => x.Category);
    }

    #endregion

    #region Salary Expense

    private async Task<Dictionary<int, decimal>> GetSalaryExpense(UnitReport grandTotalUnit)
    {
        var allEmployees = GetLastMonthReport(grandTotalUnit)?.CurrentHeadCountEmpInfo?.ToList() ?? [];
        
        if (allEmployees.Count == 0)
        {
            return new Dictionary<int, decimal>();
        }

        await payrollService.CalculateMonthlySalary(allEmployees);

        return allEmployees
            .Where(x => x.Salary.HasValue)
            .GroupBy(x => x.EmpId)
            .ToDictionary(g => g.Key, g => g.First().Salary.Value);
    }

    private static IEnumerable<HeadcountModel> GetSalaryExpenseByBUCategory(IEnumerable<UnitReport> reportByUnit, Dictionary<int, decimal> salaryByEmpDict)
    {
        return reportByUnit
            .Where(x => x.IsUnitCategory)
            .GroupBy(x => HeadcountReportHelper.ExtractCategoryName(x.Label1))
            .Select(g =>
            {
                var employees = g.SelectMany(unit => GetLastMonthReport(unit)?.CurrentHeadCountEmpInfo ?? []);
                return new HeadcountModel
                {
                    Label = g.Key,
                    Value = CalculateTotalSalary(employees, salaryByEmpDict)
                };
            });
    }

    private static IEnumerable<HeadcountModel> GetSalaryExpenseByBU(IEnumerable<UnitReport> reportByUnit, Dictionary<int, decimal> salaryByEmpDict)
    {
        return reportByUnit
            .Where(x => !x.IsGrandTotal && !x.IsUnitCategory)
            .Select(unitEmp =>
            {
                var employees = GetLastMonthReport(unitEmp)?.CurrentHeadCountEmpInfo ?? [];
                return new HeadcountModel
                {
                    Label = unitEmp.Label1,
                    Value = CalculateTotalSalary(employees, salaryByEmpDict)
                };
            });
    }

    private static decimal CalculateTotalSalary(IEnumerable<GroupEmployeeModel> employees, Dictionary<int, decimal> salaryByEmpDict) =>
        employees
            .Where(x => salaryByEmpDict.ContainsKey(x.EmpId))
            .Sum(x => salaryByEmpDict[x.EmpId]);

    #endregion

    #region Employee Statistics

    private int GetAverageTenure(IEnumerable<GroupEmployeeModel> employees)
    {
        if (employees == null || !employees.Any())
        {
            return 0;
        }

        var averageServiceYear = employees
            .Select(e => employeeService.CalculateLengthOfService(
                e.JoinedDate,
                e.MilitaryFromDate,
                e.MilitaryToDate).Years)
            .Average();

        return (int)Math.Floor(averageServiceYear);
    }

    private static (int AverageAge, IEnumerable<HeadcountModel> AgeRanges) GetEmployeeByAge(IEnumerable<GroupEmployeeModel> employees)
    {
        if (employees == null || !employees.Any())
        {
            return (0, []);
        }

        var today = DateTimeOffset.Now.Date;
        var ageBuckets = GetAgeBuckets();

        var ages = employees
            .Where(e => e.DOB != DateTimeOffset.MinValue.DateTime)
            .Select(e => CalculateAge(e.DOB, today))
            .ToList();

        if (!ages.Any())
        {
            return (0, []);
        }

        var averageAge = (int)Math.Floor(ages.Average());
        var ageRanges = ageBuckets
            .Select(bucket => new HeadcountModel
            {
                Label = bucket.Label,
                Value = ages.Count(age => age >= bucket.Min && age <= bucket.Max)
            });

        return (averageAge, ageRanges);
    }

    private static int CalculateAge(DateTime dob, DateTimeOffset today)
    {
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age).Date)
        {
            age--;
        }
        return age;
    }

    private static List<(int Min, int Max, string Label)> GetAgeBuckets() => new()
    {
        (20, 24, "20-24"),
        (25, 29, "25-29"),
        (30, 34, "30-34"),
        (35, 39, "35-39"),
        (40, 44, "40-44"),
        (45, 49, "45-49"),
        (50, 54, "50-54"),
        (55, 59, "55-59"),
        (60, 200, "60+")
    };

    #endregion
}

