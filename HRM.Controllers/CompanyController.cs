namespace HRM.Controllers;

using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using GO.UserControl.Client.Core.Enumerables;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.ViewModels.Company;
using HRM.Criteria.Company;
using HRM.Core.Constants;
using HRM.Core.Filters;

[HrmPermission(Policy.Read, FunctionKey.CompanyManagement)]
public class CompanyController(
    IUserControlSession session,
    ICompanyService companyService,
    IBaseService baseService) : BaseController(session)
{
    public ActionResult Index()
    {
        var hasPermission = session.HasPermissionOnFunction(FunctionKey.CompanyManagement, Policy.Host);
        return View(hasPermission);
    }

    [HttpPost]
    public async Task<IActionResult> GetSettingCompany()
    {
        var companies = await companyService.GetSettingCompanyAsync();

        return Json(companies);
    }

    public async Task<IActionResult> GetCurrency()
    {
        var currencies = await companyService.GetCurrencyAsync();
        return Json(currencies);
    }

    public async Task<IActionResult> GetCompanyById(int Id)
    {
        var company = await companyService.GetByIdAsync(Id);

        var model = new EditCompanySettingViewModel
        {
            CompanyId = company.CompanyId,
            Name = company.Name,
            HasBIO = company.HasBIO,
            WhitelistIPs = company.WhitelistIPs,
            Currency = company.Currency,
            EnableLetstalk = company.EnableLetstalk
        };

        return new JsonResult(model);
    }

    [HrmPermission(Policy.Read, FunctionKey.CompanyManagement)]
    public ActionResult Edit(int Id)
    {
        return View(Id);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.CompanyManagement)]
    public async Task<IActionResult> Edit([FromBody] EditCompanySettingViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid)
        {
            return new JsonResult(commandResult);
        }

        var criteria = new UpdateCompanySettingCriteria
        {
            CompanyId = model.CompanyId,
            SettingsJson = model.SettingsJson
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new JsonResult(commandResult);
    }
}