using Fanex.Data.Repository;

namespace HRM.Criteria.Company;

public class GetCompanyCriteria : CriteriaBase
{
    public override string GetSettingKey() => "CompanySetting_Get";

    public override bool IsValid() => true;
}