namespace HRM.Models.Group.ExchangeRate;

using System;

public class DeleteExchangeRate
{
    public int CurrencyId { get; set; }

    public DateTime EffectiveDate { get; set; }
}
