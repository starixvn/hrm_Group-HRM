using Fanex.Data.Repository;

namespace HRM.Criteria.Company;

public class GetCurrencyCriteria : CriteriaBase
{
    public override string GetSettingKey() => "Currency_Get";

    public override bool IsValid() => true;
}