namespace HRM.Criteria.Employee;

using Fanex.Data.Repository;

public class GetEmployeeByIdCriteria : CriteriaBase
{
    public int EmpId { get; set; }

    public override string GetSettingKey() => "Employee_GetEmployeeById";

    public override bool IsValid() => EmpId > 0;
}