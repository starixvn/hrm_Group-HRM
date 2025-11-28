using System;
using System.Collections.Generic;

namespace HRM.Services.Helper;

public static class CurrencyConverter
{
    public static decimal ConvertToUsd(decimal amount, string currencyCode, Dictionary<string, decimal> exchangeRates)
    {
        var convertedAmount = amount;

        if (!string.IsNullOrWhiteSpace(currencyCode))
        {
            var normalizedCode = currencyCode.ToUpperInvariant();
            if (exchangeRates.TryGetValue(normalizedCode, out var rate))
            {
                convertedAmount = amount * rate;
            }
        }

        return convertedAmount;
    }

    public static decimal ConvertCurrency(decimal amount, string fromCurrencyCode, string toCurrencyCode, Dictionary<string, decimal> exchangeRates)
    {
        if (string.IsNullOrWhiteSpace(fromCurrencyCode) || string.IsNullOrWhiteSpace(toCurrencyCode))
        {
            return amount;
        }

        var normalizedFrom = fromCurrencyCode.ToUpperInvariant();
        var normalizedTo = toCurrencyCode.ToUpperInvariant();

        if (normalizedFrom == normalizedTo)
        {
            return amount;
        }

        if (!exchangeRates.TryGetValue(normalizedFrom, out var fromRate) || !exchangeRates.TryGetValue(normalizedTo, out var toRate))
        {
            return amount;
        }

        var usdAmount = amount * fromRate;
        return usdAmount / toRate;
    }

    public static decimal RoundUSDAmount(decimal amount, Dictionary<string, int> roundingRules) => RoundAmount(amount, roundingRules, "USD");

    public static decimal RoundAmount(decimal amount, Dictionary<string, int> roundingRules, string currencyCode)
    {
        if (roundingRules.TryGetValue(currencyCode, out var roundingValue))
        {
            if (roundingValue >= 0)
            {
                return Math.Round(amount, roundingValue, MidpointRounding.AwayFromZero);
            }

            var roundingBase = (decimal)Math.Pow(10, Math.Abs(roundingValue));
            return Math.Round(amount / roundingBase, MidpointRounding.AwayFromZero) * roundingBase;
        }

        return amount;
    }

    public static string GetRoundingDescription(int roundingValue)
    {
        return roundingValue switch
        {
            -4 => "Rounded to nearest 1000",
            -2 => "Rounded to nearest 100",
            -1 => "Rounded to nearest 10",
            0 => "Rounded to nearest 1",
            _ => $"Rounded to {roundingValue} decimal places",
        };
    }
}
