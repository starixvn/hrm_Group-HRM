using System.Collections.Generic;
using System.Linq;
using HRM.Core.Constants;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Helper;

namespace HRM.Services.Implement.Report.Headcount;

public class UnitCountryReportCalculator(HeadcountData data) : BaseReportCalculator(data)
{
    public List<UnitReport> Calculate(bool? isLocal)
    {
        var result = new List<UnitReport>();

        var filteredEmployees = FilterEmployeesByType(Data.EmployeeInfos, isLocal);
        var wfhEmpIds = new HashSet<int>(Data.AllEmployeeWFHs.Select(wfh => wfh.EmpId));

        foreach (var categoryGroup in filteredEmployees.GroupByAndOrderByKey(x => x.UnitCategory))
        {
            foreach (var unitGroup in categoryGroup.GroupByAndOrderByKey(x => x.Unit))
            {
                var regularReports = unitGroup
                    .GroupByAndOrderByKey(e => e.CountryCode)
                    .Select(countryGroup => CreateUnitReport(unitGroup.Key, countryGroup.Key, countryGroup));

                var wfhEmployees = unitGroup.Where(e => wfhEmpIds.Contains(e.EmpId));
                var wfhReport = CreateUnitReport(unitGroup.Key, "WFH", wfhEmployees, isWFH: true);
                result.AddRangeUnitReport(regularReports);
                result.AddUnitReport(wfhReport);
            }

            result.AddRange(CalculateTotalReports(result, categoryGroup, wfhEmpIds, categoryGroup.Key, true));
        }

        result.AddRange(CalculateTotalReports(result, filteredEmployees, wfhEmpIds, HeadcountConstants.GrandTotalLabel, false));

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
                Details = CountByMonth(targetGroup, Data.AllEmployeeWFHs, isWFH),
                IsUnitCategory = isUnitCategory,
                IsGrandTotal = !isUnitCategory
            };
        }
    }
}
