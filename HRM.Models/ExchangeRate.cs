using System;

namespace HRM.Models;

public class ExchangeRate
{
    public string BaseCurrency { get; set; }

    public decimal ToUsd { get; set; }

    public DateTime EffectiveDate { get; set; }

    public decimal Rounding { get; set; }

    public DateTime ModifiedDate { get; set; }
}
