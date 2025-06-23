using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core.Constants;
using HRM.Core.Services;
using HRM.Criteria.Report.Headcount;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Interfaces.Report.Headcount;

namespace HRM.Services.Implement.Report.Headcount;

public class CalculateHeadcountService(IBaseService baseService) : IHeadcountReportService
{
    public async Task<HeadcountReportModel> GetReportAsync(int year)
    {
        var employeeInfos = await GetEmployeesInfos(year);
        var report = new HeadcountReportModel
        {
            ReportByUnit = CalculateReportByUnit(employeeInfos, year),
            ReportByCompany = CalculateReportByCompany(employeeInfos, year),
            ReportByDetails = CalculateReportByDetails(employeeInfos, year)
        };

        return report;
    }

    public async Task<List<EmployeeInfo>> GetEmployeesInfos(int year)
    {
        var criteria = new GetHeadcountReportCriteria { Year = year };

        return (await baseService.GetDataAsync<EmployeeInfo>(criteria)).ToList();
    }

    public static List<UnitReport> CalculateReportByUnit(IEnumerable<EmployeeInfo> employeeInfos, int year)
    {
        var result = employeeInfos.GroupBy(x => x.UnitCategory).OrderBy(g => g.Key)
            .SelectMany(categoryGroup =>
            {
                var unitReports = categoryGroup
                    .GroupBy(x => x.Unit).OrderBy(g => g.Key)
                    .Select(unitGroup => new UnitReport
                    {
                        Label1 = unitGroup.Key,
                        Details = CountByMonth(unitGroup, year)
                    }).ToList();

                unitReports.Add(new UnitReport
                {
                    Label1 = $"{categoryGroup.Key} {HeadcountConstants.TotalLabel}",
                    Details = CountByMonth(categoryGroup, year),
                    IsUnitCategory = true
                });

                return unitReports;
            }).ToList();

        result.Add(new UnitReport
        {
            Label1 = HeadcountConstants.GrandTotalLabel,
            Details = CountByMonth(employeeInfos, year),
            IsGrandTotal = true
        });

        return result;
    }

    public static IEnumerable<ReportModel> CountByMonth(IEnumerable<EmployeeInfo> employeesByUnit, int year)
    {
        for (var i = 1; i <= HeadcountConstants.TotalMonths; i++)
        {
            var fromDate = new DateTimeOffset(year, i, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
            var toDate = new DateTimeOffset(year, i, DateTime.DaysInMonth(year, i), 0, 0, 0, TimeSpan.Zero).DateTime;
            if (fromDate > DateTimeOffset.Now.DateTime.Date)
            {
                yield return new ReportModel();
            }
            else
            {
                yield return new ReportModel
                {
                    Month = i,
                    NewHire = employeesByUnit.Count(x => IsInMonth(x.JoinedDate, fromDate, toDate)),
                    ProbationResign = employeesByUnit.Count(x => IsInMonth(x.ResignedDate, fromDate, toDate) &&
                                                                 (x.ConfirmedDate is null ||
                                                                  x.ResignedDate <= x.ConfirmedDate)),
                    ConfirmedResign = employeesByUnit.Count(x =>
                        IsInMonth(x.ResignedDate, fromDate, toDate) && x.ResignedDate > x.ConfirmedDate),
                    CurrentHeadCount = employeesByUnit.Count(x =>
                        x.JoinedDate <= toDate && (x.ResignedDate is null || x.ResignedDate > toDate))
                };
            }
        }
    }

    public List<UnitReport> CalculateReportByCompany(IEnumerable<EmployeeInfo> employeeInfos, int year)
    {
        var result = employeeInfos.GroupBy(x => x.Country).OrderBy(g => g.Key)
            .SelectMany(countryGroup =>
            {
                var unitReports = countryGroup
                    .GroupBy(x => x.Company).OrderBy(g => g.Key).Select(companyGroup => new UnitReport
                    {
                        Label1 = countryGroup.Key,
                        Label2 = companyGroup.Key,
                        Details = CountByMonth(companyGroup, year)
                    }).ToList();

                unitReports.Add(new UnitReport
                {
                    Label1 = countryGroup.Key,
                    Label2 = HeadcountConstants.TotalLabel,
                    Details = CountByMonth(countryGroup, year),
                    IsUnitCategory = true
                });

                return unitReports;
            }).ToList();

        result.Add(new UnitReport
        {
            Label1 = HeadcountConstants.GrandTotalLabel,
            Label2 = HeadcountConstants.GrandTotalLabel,
            Details = CountByMonth(employeeInfos, year),
            IsGrandTotal = true
        });

        return result;
    }

    public List<UnitReport> CalculateReportByDetails(IEnumerable<EmployeeInfo> employeeInfos, int year)
    {
        return employeeInfos
            .GroupBy(x => new { x.Unit, x.CountryCode, x.Company })
            .OrderBy(a => a.Key.Unit)
            .ThenBy(a => a.Key.CountryCode)
            .ThenBy(a => a.Key.Company)
            .Select(group => new UnitReport
            {
                Label1 = group.Key.Unit,
                Label2 = group.Key.Company,
                Label3 = group.Key.CountryCode,
                Details = CountByMonth(group.ToList(), year)
            })
            .ToList();
    }

    private static bool IsInMonth(DateTime? date, DateTime fromDate, DateTime toDate) => date >= fromDate && date <= toDate;
}