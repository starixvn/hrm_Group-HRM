using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core;
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
        var employeeWFHs = await GetEmployeesWFH(year);
        var report = new HeadcountReportModel
        {
            ReportByUnit = CalculateReportByUnit(employeeInfos, year),
            ReportByCompany = CalculateReportByCompany(employeeInfos, year),
            ReportByDetails = CalculateReportByDetails(employeeInfos, employeeWFHs, year),
            ReportByUnitCountry = CalculateReportByUnitCountry(employeeInfos, employeeWFHs, year)
        };

        return report;
    }

    public async Task<List<EmployeeInfo>> GetEmployeesInfos(int year)
    {
        var criteria = new GetHeadcountReportCriteria { Year = year };

        return (await baseService.GetDataAsync<EmployeeInfo>(criteria)).ToList();
    }

    public async Task<List<EmployeeWFH>> GetEmployeesWFH(int year)
    {
        var criteria = new GetEmployeeWFHCriteria { Year = year };

        return (await baseService.GetDataAsync<EmployeeWFH>(criteria)).ToList();
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

    public static IEnumerable<ReportModel> CountByMonth(IEnumerable<EmployeeInfo> groupEmployees, int year, IEnumerable<EmployeeWFH> employeeWFHs = null, bool isWFH = false)
    {
        var result = new List<ReportModel>();
        for (var i = 1; i <= HeadcountConstants.TotalMonths; i++)
        {
            var fromDate = new DateTimeOffset(year, i, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
            var toDate = new DateTimeOffset(year, i, DateTime.DaysInMonth(year, i), 0, 0, 0, TimeSpan.Zero).DateTime;
            if (fromDate > DateTimeOffset.Now.DateTime.ToClientTimeZone().Date)
            {
                result.Add(new ReportModel());
            }
            else
            {
                var wfhInMonth = employeeWFHs?.Where(wfh => wfh.FromDate <= toDate && wfh.ToDate >= fromDate).Select(wfh => wfh.EmpId).ToList() ?? [];
                var employees = !isWFH
                    ? groupEmployees.Where(x => !wfhInMonth.Contains(x.EmpId))
                    : groupEmployees.Where(x => wfhInMonth.Contains(x.EmpId));
                if (!employees.Any())
                {
                    return null;
                }

                result.Add(new ReportModel
                {
                    Month = i,
                    NewHire = employees.Count(x => IsInMonth(x.JoinedDate, fromDate, toDate)),
                    ProbationResign = employees.Count(x => IsInMonth(x.ResignedDate, fromDate, toDate) &&
                                                           (x.ConfirmedDate is null ||
                                                            x.ResignedDate <= x.ConfirmedDate)),
                    ConfirmedResign = employees.Count(x =>
                        IsInMonth(x.ResignedDate, fromDate, toDate) && x.ResignedDate > x.ConfirmedDate),
                    CurrentHeadCount = employees.Count(x =>
                        x.JoinedDate <= toDate && (x.ResignedDate is null || x.ResignedDate > toDate))
                });
            }
        }

        return result.Where(x => x != null);
    }

    public static List<UnitReport> CalculateReportByCompany(IEnumerable<EmployeeInfo> employeeInfos, int year)
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

    public static List<UnitReport> CalculateReportByDetails(IEnumerable<EmployeeInfo> employeeInfos, IEnumerable<EmployeeWFH> employeeWFHs, int year)
    {
        return employeeInfos.GroupBy(x => x.Unit).OrderBy(g => g.Key)
            .SelectMany(unitGroup =>
            {
                var unitReports = new List<UnitReport>();
                var unitEmployeeIds = unitGroup.Select(e => e.EmpId);
                var wfhEmpIds = employeeWFHs
                    .Where(wfh => unitEmployeeIds.Contains(wfh.EmpId))
                    .Select(wfh => wfh.EmpId).Distinct();
                var regularReports = unitGroup
                    .GroupBy(x => x.CountryCode).OrderBy(g => g.Key)
                    .SelectMany(countryGroup => countryGroup
                        .GroupBy(x => x.Company).OrderBy(g => g.Key)
                        .Select(companyGroup =>
                        {
                            var details = CountByMonth(companyGroup, year, employeeWFHs);
                            return details == null ? null : new UnitReport
                            {
                                Label1 = unitGroup.Key,
                                Label2 = companyGroup.Key,
                                Label3 = countryGroup.Key,
                                Details = details
                            };
                        }));
                var wfhReports = unitGroup.Where(e => wfhEmpIds.Contains(e.EmpId))
                    .GroupBy(x => x.Company).OrderBy(g => g.Key)
                    .Select(companyGroup => new UnitReport
                    {
                        Label1 = unitGroup.Key,
                        Label2 = companyGroup.Key,
                        Label3 = "WFH",
                        Details = CountByMonth(companyGroup, year, employeeWFHs, true)
                    });

                unitReports.AddRange(regularReports);
                unitReports.AddRange(wfhReports);

                return unitReports.Where(x => x != null);
            }).ToList();
    }

    public static List<UnitReport> CalculateReportByUnitCountry(IEnumerable<EmployeeInfo> employeeInfos, IEnumerable<EmployeeWFH> employeeWFHs, int year)
    {
        var result = new List<UnitReport>();

        var categoryGroups = employeeInfos.GroupBy(x => x.UnitCategory).OrderBy(g => g.Key);

        foreach (var categoryGroup in categoryGroups)
        {
            var unitGroups = categoryGroup.GroupBy(x => x.Unit).OrderBy(g => g.Key);

            foreach (var unitGroup in unitGroups)
            {
                var unitEmployeeIds = unitGroup.Select(e => e.EmpId);
                var wfhEmpIds = employeeWFHs
                    .Where(wfh => unitEmployeeIds.Contains(wfh.EmpId))
                    .Select(wfh => wfh.EmpId).Distinct();

                var regularReports = unitGroup
                    .GroupBy(x => x.CountryCode).OrderBy(g => g.Key)
                    .Select(countryGroup =>
                    {
                        var details = CountByMonth(countryGroup, year, employeeWFHs);
                        return details == null ? null : new UnitReport
                        {
                            Label1 = unitGroup.Key,
                            Label3 = countryGroup.Key,
                            Details = details
                        };
                    });

                var wfhEmployees = unitGroup.Where(e => wfhEmpIds.Contains(e.EmpId));
                var wfhReports = new UnitReport
                {
                    Label1 = unitGroup.Key,
                    Label3 = "WFH",
                    Details = CountByMonth(wfhEmployees, year, employeeWFHs, true)
                };

                result.AddRange(regularReports.Where(x => x != null));
                result.Add(wfhReports);
            }

            var label3Groups = categoryGroup.GroupBy(x => x.CountryCode).OrderBy(g => g.Key);
            foreach (var label3Group in label3Groups)
            {
                result.Add(new UnitReport
                {
                    Label1 = $"{categoryGroup.Key} {HeadcountConstants.TotalLabel}",
                    Label3 = label3Group.Key,
                    Details = CountByMonth(label3Group, year, employeeWFHs),
                    IsUnitCategory = true
                });
            }
        }

        result.Add(new UnitReport
        {
            Label1 = HeadcountConstants.GrandTotalLabel,
            Details = CountByMonth(employeeInfos, year),
            IsGrandTotal = true
        });

        return result;
    }

    private static bool IsInMonth(DateTime? date, DateTime fromDate, DateTime toDate) => date >= fromDate && date <= toDate;
}