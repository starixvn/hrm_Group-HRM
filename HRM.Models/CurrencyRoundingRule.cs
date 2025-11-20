namespace HRM.Models;

public class CurrencyRoundingRule
{
    public short CurrencyId { get; set; }

    public string Currency { get; set; }

    public int Rounding { get; set; }
}
