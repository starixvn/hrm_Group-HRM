namespace HRM.Criteria.Employee;

using Fanex.Data.Repository;

public class GetEmployeeForPICCriteria : CriteriaBase
{
    public override string GetSettingKey() => "Employee_GetEmployeeForPIC";

    public override bool IsValid() => true;
}