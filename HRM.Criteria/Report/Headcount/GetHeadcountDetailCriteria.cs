using Fanex.Data.Repository;

namespace HRM.Criteria.Report.Headcount;

public class GetHeadcountDetailCriteria : CriteriaBase
{
    public string EmpIds { get; set; }
    public override string GetSettingKey() => "Report_HeadcountGet_GetDetail";

    public override bool IsValid() => true;
}
