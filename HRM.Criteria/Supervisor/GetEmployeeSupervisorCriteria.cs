namespace HRM.Criteria.Supervisor;

using Fanex.Data.Repository;

public class GetEmployeeSupervisorCriteria : CriteriaBase
{
    public int EmpId { get; set; }
    public int Role { get; set; }
    public int? CompanyId { get; set; }

    public override string GetSettingKey() => "EmployeeSupervisor_GetRequestByEmpID";

    public override bool IsValid() => EmpId > 0;
}