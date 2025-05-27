using Fanex.Data.Repository;

namespace HRM.Criteria.CompanyEmployee;

public class InsertCompanySecondmentEmployeeCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public int EmpId { get; set; }

    public int CreatedBy { get; set; }

    public override string GetSettingKey() => "CompanySecondmentEmployee_Insert";

    public override bool IsValid() => EmpId > 0 && CompanyId > 0;
}