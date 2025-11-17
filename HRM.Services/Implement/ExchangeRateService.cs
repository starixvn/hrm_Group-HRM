using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Company;
using HRM.Criteria.ExchangeRate;
using HRM.Models;
using HRM.Services.Helper;
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
        //var roundingRulesData = [];

        //return await Task.FromResult(
        //    roundingRulesData.Select(rule =>
        //    {
        //        dynamic dynamicRule = rule;
        //        return new CurrencyRoundingRule
        //        {
        //            Currency = dynamicRule.Currency ?? dynamicRule.currency ?? "",
        //            Rounding = dynamicRule.Rounding ?? dynamicRule.rounding ?? 0,
        //            Description = dynamicRule.Description ?? dynamicRule.description ?? "",
        //        };
        //    })
        //);
        return new List<CurrencyRoundingRule>();
    }

    public async Task<IEnumerable<Currency>> GetCurrenciesAsync()
    {
        var criteria = new GetCurrencyCriteria();
        return await baseService.GetDataAsync<Currency>(criteria);
    }

    public async Task<CommandResult> CreateExchangeRateAsync(
        string currencyCode,
        decimal exchangeRate,
        System.DateTime effectiveDate,
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
}
