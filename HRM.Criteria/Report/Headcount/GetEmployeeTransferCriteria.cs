using Fanex.Data.Repository;

namespace HRM.Criteria.Report.Headcount;

public class GetEmployeeTransferCriteria : CriteriaBase
{
    public int Year { get; set; }

    public override string GetSettingKey() => "Report_HeadcountGetTransferInfo";

    public override bool IsValid() => Year > 2000;
}