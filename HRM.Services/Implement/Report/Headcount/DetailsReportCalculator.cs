using System.Collections.Generic;
using System.Linq;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Helper;

namespace HRM.Services.Implement.Report.Headcount;

public class DetailsReportCalculator(HeadcountData data) : BaseReportCalculator(data)
{
    public List<UnitReport> Calculate()
    {
        return Data.EmployeeInfos.GroupByAndOrderByKey(x => x.Unit)
            .SelectMany(unitGroup =>
            {
                var unitReports = new List<UnitReport>();
                var unitEmployeeIds = unitGroup.Select(e => e.EmpId);
                var wfhEmpIds = Data.WfhEmployeeIds
                    .Where(wfh => unitEmployeeIds.Contains(wfh));
                var regularReports = unitGroup
                    .GroupByAndOrderByKey(x => x.CountryCode)
                    .SelectMany(countryGroup => countryGroup
                        .GroupByAndOrderByKey(x => x.Company)
                        .Select(companyGroup => new UnitReport
                        {
                            Label1 = unitGroup.Key,
                            Label3 = companyGroup.Key,
                            Label2 = countryGroup.Key,
                            Details = CountByMonth(companyGroup, Data.AllEmployeeWFHs)
                        }));

                var wfhReports = unitGroup.Where(e => wfhEmpIds.Contains(e.EmpId))
                    .GroupByAndOrderByKey(x => x.Company)
                    .Select(companyGroup => new UnitReport
                    {
                        Label1 = unitGroup.Key,
                        Label3 = companyGroup.Key,
                        Label2 = "WFH",
                        Details = CountByMonth(companyGroup, Data.AllEmployeeWFHs, true)
                    });

                unitReports.AddRangeUnitReport(regularReports);
                unitReports.AddRangeUnitReport(wfhReports);

                return unitReports.Where(x => x is { Details: not null } && x.Details.Any());
            }).ToList();
    }
}
