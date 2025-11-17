namespace HRM.Controllers.GroupSpace;

using System.Threading.Tasks;
using Core.Constants;
using Core.Filters;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Models;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

[HrmAuthorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class ExchangeRateController(
    IUserControlSession session,
    IExchangeRateService exchangeRateService
) : BaseController(session)
{
    [HttpGet]
    [HrmPermission(Policy.Read, FunctionKey.ExchangeRate)]
    public IActionResult Index()
    {
        var hasPermission = session.HasPermissionOnFunction(FunctionKey.ExchangeRate, Policy.Host);
        return View(hasPermission);
    }

    [HttpGet]
    [HrmPermission(Policy.Read, FunctionKey.ExchangeRate)]
    public async Task<IActionResult> GetExchangeRates()
    {
        var exchangeRates = await exchangeRateService.GetExchangeRatesAsync();
        return Json(exchangeRates);
    }

    [HttpGet]
    [HrmPermission(Policy.Read, FunctionKey.ExchangeRate)]
    public async Task<IActionResult> GetRoundingRules()
    {
        var roundingRules = await exchangeRateService.GetRoundingRulesAsync();
        return Json(roundingRules);
    }

    [HttpGet]
    [HrmPermission(Policy.Read, FunctionKey.ExchangeRate)]
    public async Task<IActionResult> GetCurrencies()
    {
        var currencies = await exchangeRateService.GetCurrenciesAsync();
        return Json(currencies);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ExchangeRate)]
    public async Task<IActionResult> Create([FromBody] CreateExchangeRate request)
    {
        if (request == null || !ModelState.IsValid)
        {
            return Json(new { code = 1, message = "Invalid request" });
        }

        var result = await exchangeRateService.CreateExchangeRateAsync(
            request.CurrencyCode,
            request.ExchangeRate,
            request.EffectiveDate,
            request.Rounding,
            EmployeeSession.EmpId
        );

        return Json(new { code = result.IsSuccessful ? 0 : result.Code, message = result.Message });
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ExchangeRate)]
    public async Task<IActionResult> Update([FromBody] UpdateExchangeRate request)
    {
        if (request == null || !ModelState.IsValid)
        {
            return Json(new { code = 1, message = "Invalid request" });
        }

        var result = await exchangeRateService.UpdateExchangeRateAsync(
            request.CurrencyId,
            request.EffectiveDate,
            request.CurrencyCode,
            request.ExchangeRate,
            request.Rounding,
            EmployeeSession.EmpId
        );

        return Json(new { code = result.IsSuccessful ? 0 : result.Code, message = result.Message });
    }
}
