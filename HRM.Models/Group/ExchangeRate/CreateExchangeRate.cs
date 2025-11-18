namespace HRM.Models.Group.ExchangeRate;

using System;

public class CreateExchangeRate
{
    public string CurrencyCode { get; set; }

    public decimal ExchangeRate { get; set; }

    public DateTime EffectiveDate { get; set; }

    public decimal Rounding { get; set; }
}
