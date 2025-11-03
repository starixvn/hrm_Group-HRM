using System;
using System.Collections.Generic;
using System.Linq;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Helper;

namespace HRM.Services.Implement.Report.Headcount;

public class BaseReportCalculator(HeadcountData data)
{
    protected readonly HeadcountData Data = data;

    protected IEnumerable<ReportModel> CountByMonth(IEnumerable<EmployeeInfo> categoryEmployees, IEnumerable<EmployeeWFH> employeeWFHs = null, bool isSeparatedWFH = false)
    {
        var empIds = categoryEmployees.Select(e => e.EmpId).ToHashSet();
        var transferIns = Data.EmployeeTransferIns.Where(t => empIds.Contains(t.EmpId)).ToList();
        var transferOuts = Data.EmployeeTransferOut.Where(t => empIds.Contains(t.EmpId)).ToList();

        var maxMonth = HeadcountReportHelper.GetMaxMonth(Data.Year);
        for (var month = 1; month <= maxMonth; month++)
        {
            var (fromDate, toDate) = GetMonthDateRange(month);
            var wfhInMonth = GetWFHEmployeesInMonth(employeeWFHs, fromDate, toDate);
            var filteredEmployees = categoryEmployees
                .Where(x => isSeparatedWFH ? wfhInMonth.Contains(x.EmpId) : !wfhInMonth.Contains(x.EmpId))
                .ToList();

            yield return CreateReportModel(month, filteredEmployees, fromDate, toDate, transferIns, transferOuts);
        }
    }

    protected static ReportModel CreateReportModel(int month, List<EmployeeInfo> employees, DateTime fromDate, DateTime toDate, 
        IEnumerable<EmployeeTransferData> transferIns, IEnumerable<EmployeeTransferData> transferOuts)
    {
        var transferInList = transferIns.Where(t => IsInRange(t.TransferDate)).Select(e => e.EmpId).ToList();
        var transferOutList = transferOuts.Where(t => IsInRange(t.TransferDate)).Select(e => e.EmpId).ToList();

        var newHireEmployees = employees.Where(e => IsInRange(e.JoinedDate)).Select(e => e.EmpId).ToList();
        var probationResignEmployees = employees.Where(e => IsInRange(e.ResignedDate) && (e.ConfirmedDate == null || e.ResignedDate <= e.ConfirmedDate)).Select(e => e.EmpId);
        var confirmedResignEmployees = employees.Where(e => IsInRange(e.ResignedDate) && e.ResignedDate > e.ConfirmedDate).Select(e => e.EmpId).ToList();
        var currentHeadCountEmployees = employees.Where(e => e.JoinedDate <= toDate && ((e.ResignedDate == null && e.Enabled) || e.ResignedDate > toDate)).Select(e => e.EmpId).Distinct().ToList();

        return new ReportModel
        {
            Month = month,
            NewHireEmpIds = newHireEmployees.Except(transferOutList).Except(transferInList),
            ProbationResignEmpIds = probationResignEmployees,
            ConfirmedResignEmpIds = confirmedResignEmployees,
            TransferInEmpIds = transferInList,
            TransferOutEmpIds = transferOutList,
            CurrentHeadCountEmpIds = currentHeadCountEmployees
        };

        bool IsInRange(DateTime? date) => date != null && IsInMonth(date.Value, fromDate, toDate);
    }

    private (DateTime fromDate, DateTime toDate) GetMonthDateRange(int month)
    {
        var fromDate = new DateTimeOffset(Data.Year, month, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
        var toDate = new DateTimeOffset(Data.Year, month, DateTime.DaysInMonth(Data.Year, month), 0, 0, 0, TimeSpan.Zero).DateTime;
        return (fromDate, toDate);
    }

    private static HashSet<int> GetWFHEmployeesInMonth(IEnumerable<EmployeeWFH> wfhRecords, DateTime fromDate, DateTime toDate)
    {
        if (wfhRecords == null)
        {
            return [];
        }

        return wfhRecords
            .Where(wfh => wfh.FromDate <= toDate && wfh.ToDate >= fromDate)
            .Select(wfh => wfh.EmpId)
            .Distinct()
            .ToHashSet();
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
