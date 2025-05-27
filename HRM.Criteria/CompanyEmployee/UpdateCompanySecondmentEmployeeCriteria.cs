using Fanex.Data.Repository;

namespace HRM.Criteria.CompanyEmployee;

public class UpdateCompanySecondmentEmployeeCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public int EmpId { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey() => "CompanySecondmentEmployee_Update";

    public override bool IsValid() => EmpId > 0 && CompanyId > 0;
}