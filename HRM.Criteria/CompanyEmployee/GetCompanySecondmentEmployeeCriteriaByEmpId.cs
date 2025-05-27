using Fanex.Data.Repository;

namespace HRM.Criteria.CompanyEmployee;

public class GetCompanySecondmentEmployeeCriteriaByEmpId : CriteriaBase
{
    public int EmpId { get; set; }

    public override string GetSettingKey() => "CompanySecondmentEmployee_GetByEmpId";

    public override bool IsValid() => EmpId > 0;
}