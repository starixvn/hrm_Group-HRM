using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Company;
using HRM.Criteria.ExchangeRate;
using HRM.Models;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class ExchangeRateService(IBaseService baseService) : IExchangeRateService
{
    public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync()
    {
        var criteria = new GetExchangeRatesCriteria();
        return await baseService.GetDataAsync<ExchangeRate>(criteria);
    }

    public async Task<IEnumerable<CurrencyRoundingRule>> GetRoundingRulesAsync()
    {
        var criteria = new GetCurrencyCriteria();
        var currencies = await baseService.GetDataAsync<Currency>(criteria);

        return currencies.Select(currency =>
        {
            var roundingValue = (int)currency.Rounding;
            return new CurrencyRoundingRule
            {
                Currency = currency.CurrencyCode,
                Rounding = roundingValue,
                Description = GetRoundingDescription(roundingValue),
            };
        });
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

    public async Task<IEnumerable<Currency>> GetCurrenciesAsync()
    {
        var criteria = new GetCurrencyCriteria();
        return await baseService.GetDataAsync<Currency>(criteria);
    }

    public async Task<CommandResult> CreateExchangeRateAsync(
        string currencyCode,
        decimal exchangeRate,
        DateTime effectiveDate,
        decimal rounding,
        int modifiedBy
    )
    {
        var criteria = new InsertExchangeRateCriteria
        {
            CurrencyCode = currencyCode,
            ExchangeRate = exchangeRate,
            EffectiveDate = effectiveDate,
            Rounding = rounding,
            ModifiedBy = modifiedBy,
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> UpdateExchangeRateAsync(
        int currencyId,
        DateTime effectiveDate,
        DateTime oldEffectiveDate,
        string currencyCode,
        decimal exchangeRate,
        decimal rounding,
        int modifiedBy
    )
    {
        var criteria = new UpdateExchangeRateCriteria
        {
            CurrencyId = currencyId,
            EffectiveDate = effectiveDate,
            OldEffectiveDate = oldEffectiveDate,
            CurrencyCode = currencyCode,
            ExchangeRate = exchangeRate,
            Rounding = rounding,
            ModifiedBy = modifiedBy,
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> DeleteExchangeRateAsync(
        int currencyId,
        DateTime effectiveDate,
        int modifiedBy
    )
    {
        var criteria = new DeleteExchangeRateCriteria
        {
            CurrencyId = currencyId,
            EffectiveDate = effectiveDate,
            ModifiedBy = modifiedBy,
        };

        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}
