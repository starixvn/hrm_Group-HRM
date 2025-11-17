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
        { "VND", 0.00004000m },
    };

    private static readonly Dictionary<string, int> CurrencyRoundingRules = new()
    {
        { "USD", -1 },
        { "VND", -4 },
        { "PHP", -2 },
        { "SGD", -1 },
        { "MYR", -1 },
        { "NTD", -1 },
    };

    public static decimal ConvertToUsd(decimal amount, string currencyCode)
    {
        var convertedAmount = amount;

        if (!string.IsNullOrWhiteSpace(currencyCode))
        {
            var normalizedCode = currencyCode.ToUpperInvariant();
            if (CurrencyToUsdRates.TryGetValue(normalizedCode, out var rate))
            {
                convertedAmount = amount * rate;
            }
        }

        return convertedAmount;
    }

    public static decimal RoundUSDAmount(decimal amount) => RoundAmount(amount, "USD");

    public static decimal RoundAmount(decimal amount, string currencyCode)
    {
        if (CurrencyRoundingRules.TryGetValue(currencyCode, out var roundingValue))
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

    public static Dictionary<string, decimal> GetExchangeRates()
    {
        return new Dictionary<string, decimal>(CurrencyToUsdRates);
    }

    public static List<object> GetRoundingRules()
    {
        var roundingRules = new List<object>();

        foreach (var rule in CurrencyRoundingRules)
        {
            var description = GetRoundingDescription(rule.Value);
            roundingRules.Add(
                new
                {
                    Currency = rule.Key,
                    Rounding = rule.Value,
                    Description = description,
                }
            );
        }

        roundingRules.Add(
            new
            {
                Currency = "Others",
                Rounding = 0,
                Description = "Rounded to nearest 1",
            }
        );

        return roundingRules;
    }

    private static string GetRoundingDescription(int roundingValue)
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
