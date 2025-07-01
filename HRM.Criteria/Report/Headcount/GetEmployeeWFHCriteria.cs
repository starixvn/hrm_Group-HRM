using Fanex.Data.Repository;

namespace HRM.Criteria.Report.Headcount;

public class GetEmployeeWFHCriteria : CriteriaBase
{
    public int Year { get; set; }

    public override string GetSettingKey() => "Report_HeadcountGetWFH";

    public override bool IsValid() => Year > 2000;
}