using System.Collections.Generic;
using System.Linq;
using HRM.Core.Constants;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Helper;

namespace HRM.Services.Implement.Report.Headcount;

public class CompanyReportCalculator(HeadcountData data) : BaseReportCalculator(data)
{
    public List<UnitReport> Calculate()
    {
        var result = Data.EmployeeInfos.GroupByAndOrderByKey(x => x.Country)
            .SelectMany(countryGroup =>
            {
                var unitReports = countryGroup
                    .GroupByAndOrderByKey(x => x.Company).Select(companyGroup => new UnitReport
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
            Details = CountByMonth(Data.EmployeeInfos),
            IsGrandTotal = true
        });

        return result;
    }
}
