namespace HRM.Criteria.Department;

using Fanex.Data.Repository;

public class GetNextGroupDeptIdCriteria : CriteriaBase
{
    public override string GetSettingKey() => "Department_GetNextGroupDeptId";

    public override bool IsValid() => true;
}