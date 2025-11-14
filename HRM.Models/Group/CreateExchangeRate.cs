namespace HRM.Models;

using System;

public class CreateExchangeRate
{
    public string CurrencyCode { get; set; }

    public decimal ExchangeRate { get; set; }

    public DateTime EffectiveDate { get; set; }
}
