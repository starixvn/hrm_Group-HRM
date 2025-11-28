using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core;
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

        return currencies.Select(currency => new CurrencyRoundingRule
        {
            CurrencyId = (short)currency.Id,
            Currency = currency.CurrencyCode,
            Rounding = (int)currency.Rounding,
        });
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
        int modifiedBy
    )
    {
        var criteria = new InsertExchangeRateCriteria
        {
            CurrencyCode = currencyCode,
            ExchangeRate = exchangeRate,
            EffectiveDate = effectiveDate,
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

    public async Task<CommandResult> UpdateCurrencyRoundingAsync(
        short currencyId,
        int rounding,
        int modifiedBy
    )
    {
        var criteria = new UpdateCurrencyRoundingCriteria
        {
            CurrencyId = currencyId,
            Rounding = rounding,
            ModifiedBy = modifiedBy,
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<Dictionary<string, decimal>> GetEffectiveExchangeRatesDictAsync()
    {
        var currentDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        var exchangeRates = await GetExchangeRatesAsync();

        var effectiveRates = exchangeRates
            .Where(er => er.EffectiveDate.Date <= currentDate)
            .GroupBy(er => er.BaseCurrency.ToUpperInvariant())
            .Select(g => g.OrderByDescending(er => er.EffectiveDate).First())
            .ToList();

        var ratesDict = effectiveRates.ToDictionary(
            er => er.BaseCurrency.ToUpperInvariant(),
            er => er.ToUsd
        );

        return ratesDict;
    }
}
