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
        System.DateTime effectiveDate,
        int modifiedBy
    );
}
