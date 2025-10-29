namespace HRM.Criteria.EmployeePayrollSetting;

using Fanex.Data.Repository;

public class GetEmployeePayrollAllowanceByEmpIdCriteria : CriteriaBase
{
    public int EmpId { get; set; }

    public override string GetSettingKey()
    {
        return "EmployeePayrollAllowance_GetByEmpId";
    }

    public override bool IsValid()
    {
        return EmpId > 0;
    }
}