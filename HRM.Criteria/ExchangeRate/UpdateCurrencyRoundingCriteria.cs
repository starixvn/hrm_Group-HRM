namespace HRM.Criteria.ExchangeRate;

using Fanex.Data.Repository;

public class UpdateCurrencyRoundingCriteria : CriteriaBase
{
    public short CurrencyId { get; set; }

    public int Rounding { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey() => "Currency_UpdateRounding";

    public override bool IsValid() => CurrencyId > 0;
}
