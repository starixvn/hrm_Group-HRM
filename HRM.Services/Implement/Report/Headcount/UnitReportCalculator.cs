using System.Collections.Generic;
using System.Linq;
using HRM.Core.Constants;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Helper;

namespace HRM.Services.Implement.Report.Headcount;

public class UnitReportCalculator(HeadcountData data) : BaseReportCalculator(data)
{
    public List<UnitReport> Calculate()
    {
        var result = Data.EmployeeInfos.GroupByAndOrderByKey(x => x.UnitCategory)
            .SelectMany(categoryGroup =>
            {
                var unitReports = categoryGroup
                    .GroupByAndOrderByKey(x => x.Unit)
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
            Details = CountByMonth(Data.EmployeeInfos),
            IsGrandTotal = true
        });

        return result;
    }
}
