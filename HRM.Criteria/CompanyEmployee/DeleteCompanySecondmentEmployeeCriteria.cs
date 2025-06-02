using Fanex.Data.Repository;

namespace HRM.Criteria.CompanyEmployee;

public class DeleteCompanySecondmentEmployeeCriteria : CriteriaBase
{
    public int EmpId { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey() => "CompanySecondmentEmployee_Delete";

    public override bool IsValid() => EmpId > 0;
}