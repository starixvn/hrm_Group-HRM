using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public List<UnitReport> CalculateReportByUnit(IEnumerable<EmployeeInfo> employeeInfos, int year)
    {
        var result = employeeInfos
            .GroupBy(x => x.UnitCategory)
            .OrderBy(g => g.Key)
            .SelectMany(categoryGroup =>
            {
                var unitReports = categoryGroup
                    .GroupBy(x => x.Unit)
                    .OrderBy(g => g.Key)
                    .Select(unitGroup => new UnitReport
                    {
                        Unit = unitGroup.Key,
                        Details = CountByMonth(unitGroup.ToList(), year)
                    })
                    .ToList();

                unitReports.Add(new UnitReport
                {
                    Unit = $"{categoryGroup.Key} TOTAL",
                    Details = CountByMonth(categoryGroup.ToList(), year),
                    IsUnitCategory = true
                });

                return unitReports;
            })
            .ToList();
        return result;
    }

    public static IEnumerable<ReportModel> CountByMonth(List<EmployeeInfo> employeesByUnit, int year)
    {
        for (var i = 1; i <= 12; i++)
        {
            var fromDate = new DateTime(year, i, 1);
            var toDate = new DateTime(year, i, DateTime.DaysInMonth(year, i));
            if (fromDate > DateTime.Now.Date)
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
        return employeeInfos
            .GroupBy(x => new { x.Country, x.Company })
            .OrderBy(a => a.Key.Country).ThenBy(a => a.Key.Company)
            .Select(group => new UnitReport
            {
                Company = group.Key.Company,
                Country = group.Key.Country,
                Details = CountByMonth(group.ToList(), year)
            })
            .ToList();
    }

    public List<UnitReport> CalculateReportByDetails(IEnumerable<EmployeeInfo> employeeInfos, int year)
    {
        return employeeInfos
            .GroupBy(x => new { x.Unit, x.LocationCode, x.Company, x.UserType })
            .OrderBy(a => a.Key.Unit)
            .ThenBy(a => a.Key.LocationCode)
            .ThenBy(a => a.Key.Company)
            .ThenBy(a => a.Key.UserType)
            .Select(group => new UnitReport
            {
                Company = $"{group.Key.UserType} {group.Key.Company}",
                Unit = group.Key.Unit,
                LocationCode = group.Key.LocationCode,
                Details = CountByMonth(group.ToList(), year)
            })
            .ToList();
    }

    private static bool IsInMonth(DateTime? date, DateTime fromDate, DateTime toDate) => date >= fromDate && date <= toDate;
}