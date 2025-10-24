using System;
using System.Collections.Generic;
using System.Linq;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Helper;

namespace HRM.Services.Implement.Report.Headcount;

public class BaseReportCalculator(HeadcountData data)
{
    protected readonly HeadcountData Data = data;

    protected IEnumerable<ReportModel> CountByMonth(IEnumerable<EmployeeInfo> groupEmployees, IEnumerable<EmployeeWFH> employeeWFHs = null, bool isWFH = false)
    {
        var maxMonth = HeadcountReportHelper.GetMaxMonth(Data.Year);
        for (var i = 1; i <= maxMonth; i++)
        {
            var fromDate = new DateTimeOffset(Data.Year, i, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
            var toDate = new DateTimeOffset(Data.Year, i, DateTime.DaysInMonth(Data.Year, i), 0, 0, 0, TimeSpan.Zero).DateTime;

            var wfhInMonth = employeeWFHs?.Where(wfh => wfh.FromDate <= toDate && wfh.ToDate >= fromDate).Select(wfh => wfh.EmpId).ToList() ?? [];
            var employees = isWFH
                ? groupEmployees.Where(x => wfhInMonth.Contains(x.EmpId))
                : groupEmployees.Where(x => !wfhInMonth.Contains(x.EmpId));

            yield return CreateReportModel(i, employees.ToList(), fromDate, toDate);
        }
    }

    protected static ReportModel CreateReportModel(int month, IEnumerable<EmployeeInfo> employees, DateTime fromDate, DateTime toDate)
    {
        return new ReportModel
        {
            Month = month,
            NewHire = employees.Count(x => IsInMonth(x.JoinedDate, fromDate, toDate)),
            ProbationResign = employees.Count(x => IsInMonth(x.ResignedDate, fromDate, toDate) &&
                                                    (x.ConfirmedDate is null || x.ResignedDate <= x.ConfirmedDate)),
            ConfirmedResign = employees.Count(x =>
                IsInMonth(x.ResignedDate, fromDate, toDate) && x.ResignedDate > x.ConfirmedDate),
            CurrentHeadCount = employees.Count(x =>
                x.JoinedDate <= toDate && (x.ResignedDate is null || x.ResignedDate > toDate))
        };
    }

    protected static bool IsInMonth(DateTime? date, DateTime fromDate, DateTime toDate) => date >= fromDate && date <= toDate;

    protected UnitReport CreateUnitReport(string label1, string label3, IEnumerable<EmployeeInfo> group, bool isWFH = false) =>
        new()
        {
            Label1 = label1,
            Label3 = label3,
            Details = CountByMonth(group, Data.AllEmployeeWFHs, isWFH)
        };

    protected static IEnumerable<EmployeeInfo> FilterEmployeesByType(IEnumerable<EmployeeInfo> employees, bool? isLocal)
        => isLocal switch
        {
            null => employees,
            true => employees.Where(e => e.IsLocal),
            false => employees.Where(e => !e.IsLocal)
        };
}
