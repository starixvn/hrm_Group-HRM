using Fanex.Data.Repository;

namespace HRM.Criteria.Supervisor;

public class GetDisciplineSettingByEmployeeId : CriteriaBase
{
    public int EmpId { get; set; }
    public int? CompanyId { get; set; }

    public override bool IsValid() => EmpId > 0;

    public override string GetSettingKey() => "EmployeeSupervisor_GetDisciplineSettingById";
}