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
    private IEnumerable<EmployeeWFH> allEmployeeWFHs;
    private int year;
    private IEnumerable<EmployeeInfo> employeeInfos;

    public async Task<HeadcountReportModel> GetReportAsync(int selectedYear)
    {
        year = selectedYear;
        employeeInfos = await GetEmployeesInfos(year);
        allEmployeeWFHs = await GetEmployeesWFH(year);
        var report = new HeadcountReportModel
        {
            ReportByUnit = CalculateReportByUnit(),
            ReportByCompany = CalculateReportByCompany(),
            ReportByDetails = CalculateReportByDetails(),
            ReportByUnitCountry = CalculateReportByUnitCountry()
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

    public List<UnitReport> CalculateReportByUnit()
    {
        var result = employeeInfos.GroupBy(x => x.UnitCategory).OrderBy(g => g.Key)
            .SelectMany(categoryGroup =>
            {
                var unitReports = categoryGroup
                    .GroupBy(x => x.Unit).OrderBy(g => g.Key)
                    .Select(unitGroup => new UnitReport
                    {
                        Label1 = unitGroup.Key,
                        Details = CountByMonth(unitGroup)
                    }).ToList();

                unitReports.Add(new UnitReport
                {
                    Label1 = $"{categoryGroup.Key} {HeadcountConstants.TotalLabel}",
                    Details = CountByMonth(categoryGroup),
                    IsUnitCategory = true
                });

                return unitReports;
            }).ToList();

        result.Add(new UnitReport
        {
            Label1 = HeadcountConstants.GrandTotalLabel,
            Details = CountByMonth(employeeInfos),
            IsGrandTotal = true
        });

        return result;
    }

    public IEnumerable<ReportModel> CountByMonth(IEnumerable<EmployeeInfo> groupEmployees, IEnumerable<EmployeeWFH> employeeWFHs = null, bool isWFH = false)
    {
        var result = new List<ReportModel>();
        for (var i = 1; i <= HeadcountConstants.TotalMonths; i++)
        {
            var fromDate = new DateTimeOffset(year, i, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
            var toDate = new DateTimeOffset(year, i, DateTime.DaysInMonth(year, i), 0, 0, 0, TimeSpan.Zero).DateTime;

            if (fromDate > DateTimeOffset.Now.DateTime.ToClientTimeZone().Date)
            {
                result.Add(new ReportModel());
                continue;
            }

            var employees = GetFilteredEmployees(groupEmployees, employeeWFHs, isWFH, fromDate, toDate);
            if (!employees.Any())
            {
                return [];
            }

            result.Add(CreateReportModel(i, employees, fromDate, toDate));
        }

        return result;
    }

    private static IEnumerable<EmployeeInfo> GetFilteredEmployees(IEnumerable<EmployeeInfo> groupEmployees, IEnumerable<EmployeeWFH> employeeWFHs, bool isWFH, DateTime fromDate, DateTime toDate)
    {
        var wfhInMonth = employeeWFHs?.Where(wfh => wfh.FromDate <= toDate && wfh.ToDate >= fromDate).Select(wfh => wfh.EmpId).ToList() ?? [];

        return !isWFH
            ? groupEmployees.Where(x => !wfhInMonth.Contains(x.EmpId))
            : groupEmployees.Where(x => wfhInMonth.Contains(x.EmpId));
    }

    private static ReportModel CreateReportModel(int month, IEnumerable<EmployeeInfo> employees, DateTime fromDate, DateTime toDate)
    {
        return new ReportModel
        {
            Month = month,
            NewHire = employees.Count(x => IsInMonth(x.JoinedDate, fromDate, toDate)),
            ProbationResign = employees.Count(x => IsInMonth(x.ResignedDate, fromDate, toDate) &&
                                                    (x.ConfirmedDate is null ||
                                                    x.ResignedDate <= x.ConfirmedDate)),
            ConfirmedResign = employees.Count(x =>
                IsInMonth(x.ResignedDate, fromDate, toDate) && x.ResignedDate > x.ConfirmedDate),
            CurrentHeadCount = employees.Count(x =>
                x.JoinedDate <= toDate && (x.ResignedDate is null || x.ResignedDate > toDate))
        };
    }

    public List<UnitReport> CalculateReportByCompany()
    {
        var result = employeeInfos.GroupBy(x => x.Country).OrderBy(g => g.Key)
            .SelectMany(countryGroup =>
            {
                var unitReports = countryGroup
                    .GroupBy(x => x.Company).OrderBy(g => g.Key).Select(companyGroup => new UnitReport
                    {
                        Label1 = countryGroup.Key,
                        Label2 = companyGroup.Key,
                        Details = CountByMonth(companyGroup)
                    }).ToList();

                unitReports.Add(new UnitReport
                {
                    Label1 = countryGroup.Key,
                    Label2 = HeadcountConstants.TotalLabel,
                    Details = CountByMonth(countryGroup),
                    IsUnitCategory = true
                });

                return unitReports;
            }).ToList();

        result.Add(new UnitReport
        {
            Label1 = HeadcountConstants.GrandTotalLabel,
            Label2 = HeadcountConstants.GrandTotalLabel,
            Details = CountByMonth(employeeInfos),
            IsGrandTotal = true
        });

        return result;
    }

    public List<UnitReport> CalculateReportByDetails()
    {
        return employeeInfos.GroupBy(x => x.Unit).OrderBy(g => g.Key)
            .SelectMany(unitGroup =>
            {
                var unitReports = new List<UnitReport>();
                var unitEmployeeIds = unitGroup.Select(e => e.EmpId);
                var wfhEmpIds = allEmployeeWFHs
                    .Where(wfh => unitEmployeeIds.Contains(wfh.EmpId))
                    .Select(wfh => wfh.EmpId).Distinct();
                var regularReports = unitGroup
                    .GroupBy(x => x.CountryCode).OrderBy(g => g.Key)
                    .SelectMany(countryGroup => countryGroup
                        .GroupBy(x => x.Company).OrderBy(g => g.Key)
                        .Select(companyGroup =>
                        {
                            var details = CountByMonth(companyGroup, allEmployeeWFHs);
                            return !details.Any() ? null : new UnitReport
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
                        Details = CountByMonth(companyGroup, allEmployeeWFHs, true)
                    });

                unitReports.AddRange(regularReports);
                unitReports.AddRange(wfhReports);

                return unitReports.Where(x => x is { Details: not null } && x.Details.Any());
            }).ToList();
    }

    public List<UnitReport> CalculateReportByUnitCountry()
    {
        var result = new List<UnitReport>();
        var wfhEmpIds = new HashSet<int>(allEmployeeWFHs.Select(wfh => wfh.EmpId));

        foreach (var categoryGroup in employeeInfos.GroupBy(x => x.UnitCategory))
        {
            foreach (var unitGroup in categoryGroup.GroupBy(x => x.Unit))
            {
                var regularReports = unitGroup
                    .Where(e => !wfhEmpIds.Contains(e.EmpId))
                    .GroupBy(e => e.CountryCode)
                    .Select(countryGroup => CreateUnitReport(unitGroup.Key, countryGroup.Key, countryGroup))
                    .Where(report => report != null);

                result.AddRange(regularReports);

                var wfhEmployees = unitGroup.Where(e => wfhEmpIds.Contains(e.EmpId));
                var wfhReport = CreateUnitReport(unitGroup.Key, "WFH", wfhEmployees, isWFH: true);
                if (wfhReport != null)
                {
                    result.Add(wfhReport);
                }
            }

            result.AddRange(CalculateTotalReports(result, categoryGroup, wfhEmpIds, categoryGroup.Key, true));
        }

        result.AddRange(CalculateTotalReports(result, employeeInfos, wfhEmpIds, HeadcountConstants.GrandTotalLabel, false));

        return result;
    }

    private IEnumerable<UnitReport> CalculateTotalReports(List<UnitReport> result, IEnumerable<EmployeeInfo> group, HashSet<int> wfhEmpIds, string label1, bool isUnitCategory)
    {
        var label3List = result.Select(r => r.Label3).Distinct().ToList();
        foreach (var label3 in label3List)
        {
            var isWFH = label3 == "WFH";
            var targetGroup = isWFH
                ? group.Where(e => wfhEmpIds.Contains(e.EmpId))
                : group.Where(e => e.CountryCode == label3);

            yield return new UnitReport
            {
                Label1 = label1,
                Label3 = label3,
                Details = CountByMonth(targetGroup, allEmployeeWFHs, isWFH),
                IsUnitCategory = isUnitCategory,
                IsGrandTotal = !isUnitCategory
            };
        }
    }

    private UnitReport CreateUnitReport(string label1, string label3,
        IEnumerable<EmployeeInfo> group, bool isWFH = false)
    {
        if (!group.Any())
        {
            return null;
        }
        var details = CountByMonth(group, allEmployeeWFHs, isWFH);
        return !details.Any() ? null : new UnitReport
        {
            Label1 = label1,
            Label3 = label3,
            Details = details
        };
    }

    private static bool IsInMonth(DateTime? date, DateTime fromDate, DateTime toDate) => date >= fromDate && date <= toDate;
}