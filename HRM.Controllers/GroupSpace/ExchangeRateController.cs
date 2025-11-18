namespace HRM.Controllers.GroupSpace;

using System.Threading.Tasks;
using Core.Constants;
using Core.Filters;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Models;
using HRM.Models.Group.ExchangeRate;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ExchangeRate)]
    public async Task<IActionResult> Create([FromBody] CreateExchangeRate model)
    {
        if (model == null || !ModelState.IsValid)
        {
            return Json(new { code = 1, message = "Invalid request" });
        }

        var result = await exchangeRateService.CreateExchangeRateAsync(
            model.CurrencyCode,
            model.ExchangeRate,
            model.EffectiveDate,
            model.Rounding,
            EmployeeSession.EmpId
        );

        return Json(new { code = result.IsSuccessful ? 0 : result.Code, message = result.Message });
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ExchangeRate)]
    public async Task<IActionResult> Update([FromBody] UpdateExchangeRate model)
    {
        if (model == null || !ModelState.IsValid)
        {
            return Json(new { code = 1, message = "Invalid request" });
        }

        var result = await exchangeRateService.UpdateExchangeRateAsync(
            model.CurrencyId,
            model.EffectiveDate,
            model.OldEffectiveDate,
            model.CurrencyCode,
            model.ExchangeRate,
            model.Rounding,
            EmployeeSession.EmpId
        );

        return Json(new { code = result.IsSuccessful ? 0 : result.Code, message = result.Message });
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ExchangeRate)]
    public async Task<IActionResult> Delete([FromBody] DeleteExchangeRate model)
    {
        if (model == null || !ModelState.IsValid)
        {
            return Json(new { code = 1, message = "Invalid request" });
        }

        var result = await exchangeRateService.DeleteExchangeRateAsync(
            model.CurrencyId,
            model.EffectiveDate,
            EmployeeSession.EmpId
        );

        return Json(new { code = result.IsSuccessful ? 0 : result.Code, message = result.Message });
    }
}
