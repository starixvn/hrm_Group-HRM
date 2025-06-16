using Fanex.Data.Repository;

namespace HRM.Criteria.Report.Headcount;

public class GetHeadcountReportCriteria : CriteriaBase
{
    public int Year { get; set; }

    public override string GetSettingKey() => "Report_HeadcountGet";

    public override bool IsValid() => Year > 2000;
}