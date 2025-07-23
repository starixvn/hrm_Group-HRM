using Fanex.Data.Repository;
using HRM.Models;

namespace HRM.Criteria.Group;

public class GetCompaniesByRoleIdForEmployeeCriteria : CriteriaBase
{
    public int EmpId { get; set; }
    public int RoleId { get; set; }

    public override bool IsValid() => EmpId > 0;

    public override string GetSettingKey() => "GroupSupervisor_GetCompaniesByRoleIdForEmployee";
}