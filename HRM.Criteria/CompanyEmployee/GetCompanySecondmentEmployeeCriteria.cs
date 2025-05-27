using Fanex.Data.Repository;

namespace HRM.Criteria.CompanyEmployee;

public class GetCompanySecondmentEmployeeCriteria : CriteriaBase
{
    public override string GetSettingKey() => "CompanySecondmentEmployee_Get";

    public override bool IsValid() => true;
}