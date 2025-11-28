using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Models;

namespace HRM.Services.Interfaces;

public interface IExchangeRateService
{
    Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync();

    Task<IEnumerable<CurrencyRoundingRule>> GetRoundingRulesAsync();

    Task<IEnumerable<Currency>> GetCurrenciesAsync();

    Task<CommandResult> CreateExchangeRateAsync(
        string currencyCode,
        decimal exchangeRate,
        DateTime effectiveDate,
        int modifiedBy
    );

    Task<CommandResult> UpdateExchangeRateAsync(
        int currencyId,
        DateTime effectiveDate,
        DateTime oldEffectiveDate,
        string currencyCode,
        decimal exchangeRate,
        int modifiedBy
    );

    Task<CommandResult> DeleteExchangeRateAsync(
        int currencyId,
        DateTime effectiveDate,
        int modifiedBy
    );

    Task<CommandResult> UpdateCurrencyRoundingAsync(short currencyId, int rounding, int modifiedBy);

    Task<Dictionary<string, decimal>> GetEffectiveExchangeRatesDictAsync();
}
