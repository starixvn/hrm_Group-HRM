using System;
using System.Collections.Generic;

namespace HRM.Services.Helper;

public static class CurrencyConverter
{
    private static readonly Dictionary<string, decimal> CurrencyToUsdRates = new()
    {
        { "USD", 1.0m },
        { "MYR", 0.23202708m },
        { "NTD", 0.03058717m },
        { "CNY", 0.13862883m },
        { "THB", 0.03039631m },
        { "GBP", 1.33330459m },
        { "EUR", 1.13825178m },
        { "PHP", 0.01725000m },
        { "SGD", 0.76986547m },
        { "VND", 0.00004000m }
    };

    private static readonly Dictionary<string, int> CurrencyRoundingRules = new()
    {
        { "USD", -1 },
        { "VND", -4 },
        { "PHP", -2 },
        { "SGD", -1 },
        { "MYR", -1 },
        { "NTD", -1 }
    };

    public static decimal ConvertToUsd(decimal amount, string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            return amount;
        }

        var normalizedCode = currencyCode.ToUpperInvariant();
        if (!CurrencyToUsdRates.TryGetValue(normalizedCode, out var rate))
        {
            return amount;
        }

        var convertedAmount = amount * rate;
        
        if (CurrencyRoundingRules.TryGetValue("USD", out var roundingValue))
        {
            convertedAmount = RoundAmount(convertedAmount, roundingValue);
        }

        return convertedAmount;
    }

    private static decimal RoundAmount(decimal amount, int roundingValue)
    {
        if (roundingValue >= 0)
        {
            return Math.Round(amount, roundingValue, MidpointRounding.AwayFromZero);
        }

        var roundingBase = (decimal)Math.Pow(10, Math.Abs(roundingValue));
        return Math.Round(amount / roundingBase, MidpointRounding.AwayFromZero) * roundingBase;
    }
}

