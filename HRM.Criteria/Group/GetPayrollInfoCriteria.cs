using Fanex.Data.Repository;

namespace HRM.Criteria.Group;

public class GetPayrollInfoCriteria : CriteriaBase
{
    public string EmpIds { get; set; }

    public override string GetSettingKey() => "Employee_GroupScope_GetPayrollInfo";

    public override bool IsValid() => true;
}