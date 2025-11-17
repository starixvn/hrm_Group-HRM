namespace HRM.Criteria.ExchangeRate;

using System;
using Fanex.Data.Repository;

public class UpdateExchangeRateCriteria : CriteriaBase
{
    public int CurrencyId { get; set; }

    public DateTime EffectiveDate { get; set; }

    public string CurrencyCode { get; set; }

    public decimal ExchangeRate { get; set; }

    public decimal Rounding { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey() => "CurrencyExchangeRate_Update";

    public override bool IsValid() =>
        CurrencyId > 0 && !string.IsNullOrWhiteSpace(CurrencyCode) && ExchangeRate > 0 && EffectiveDate != default;
}
