namespace HRM.Criteria.ExchangeRate;

using System;
using Fanex.Data.Repository;

public class InsertExchangeRateCriteria : CriteriaBase
{
    public string CurrencyCode { get; set; }

    public decimal ExchangeRate { get; set; }

    public DateTime EffectiveDate { get; set; }

    public int ModifiedBy { get; set; }

    public override string GetSettingKey() => "CurrencyExchangeRate_Insert";

    public override bool IsValid() =>
        !string.IsNullOrWhiteSpace(CurrencyCode) && ExchangeRate > 0 && EffectiveDate != default;
}
