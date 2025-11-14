namespace HRM.Criteria.ExchangeRate;

using Fanex.Data.Repository;

public class GetExchangeRatesCriteria : CriteriaBase
{
    public override string GetSettingKey() => "CurrencyExchangeRate_Get";

    public override bool IsValid() => true;
}
