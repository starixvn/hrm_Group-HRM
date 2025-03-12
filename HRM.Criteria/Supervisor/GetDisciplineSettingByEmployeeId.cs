namespace HRM.Criteria;

using Fanex.Data.Repository;

public class GetDisciplineSettingByEmployeeId : CriteriaBase
{
    public int EmpId { get; set; }

    public override bool IsValid() => EmpId > 0;

    public override string GetSettingKey() => "EmployeeSupervisor_GetDisciplineSettingById";
}