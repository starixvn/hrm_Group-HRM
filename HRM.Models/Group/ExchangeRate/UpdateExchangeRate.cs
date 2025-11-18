namespace HRM.Models.Group.ExchangeRate;

using System;

public class UpdateExchangeRate
{
    public int CurrencyId { get; set; }

    public DateTime EffectiveDate { get; set; }

    public DateTime OldEffectiveDate { get; set; }

    public string CurrencyCode { get; set; }

    public decimal ExchangeRate { get; set; }

    public decimal Rounding { get; set; }
}
