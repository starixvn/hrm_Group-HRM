namespace HRM.Criteria.ExchangeRate;

using System;
using Fanex.Data.Repository;

public class DeleteExchangeRateCriteria : CriteriaBase
{
    public int CurrencyId { get; set; }
    public DateTime EffectiveDate { get; set; }
    public int ModifiedBy { get; set; }

    public override string GetSettingKey()
    {
        return "CurrencyExchangeRate_Delete";
    }

    public override bool IsValid()
    {
        return CurrencyId > 0;
    }
}
