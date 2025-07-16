using Fanex.Data.Repository;

namespace HRM.Criteria.Supervisor;

public class GetSupervisorCompaniesCriteria : CriteriaBase
{
    public int EmpId { get; set; }

    public override string GetSettingKey() => "SupervisorCompanies_GetByEmpId";

    public override bool IsValid() => true;
}