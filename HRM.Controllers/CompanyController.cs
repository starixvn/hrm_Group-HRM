namespace HRM.Controllers;

using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Company;
using HRM.ViewModels.Company;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

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
        var subCompanies = await companyService.GetSubCompanies(null);

        foreach (var company in companies)
        {
            company.SubCompanyList = subCompanies
                .Where(sub => sub.CompanyId == company.CompanyId)
                .ToList();
        }

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
        var subCompanies = await companyService.GetSubCompanies(Id);
        var countEmployee = await companyService.CountEmployeeById(Id);

        foreach (var sub in subCompanies)
        {
            var countItem = countEmployee.FirstOrDefault(x => x.SubCompanyId == sub.SubCompanyId);
            sub.EmployeeCount = countItem?.EmployeeCount ?? 0;
        }

        var model = new EditCompanySettingViewModel
        {
            CompanyId = company.CompanyId,
            Name = company.Name,
            HasBIO = company.HasBIO,
            WhitelistIPs = company.WhitelistIPs,
            Urls = company.Urls,
            Currency = company.Currency,
            EnableLetstalk = company.EnableLetstalk,
            SubCompanies = subCompanies.ToList()
        };

        return Json(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.CompanyManagement)]
    public ActionResult Edit(int Id)
    {
        return View(Id);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.CompanyManagement)]
    public async Task<IActionResult> Edit([FromBody] EditCompanySettingViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid)
        {
            return Json(commandResult);
        }

        var criteria = new UpdateCompanySettingCriteria
        {
            CompanyId = model.CompanyId,
            SettingsJson = model.SettingsJson
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        if (!commandResult.IsSuccessful)
        {
            return Json(commandResult);
        }

        foreach (var sub in model.SubCompanies)
        {
            switch (sub.ActionType?.ToLower())
            {
                case "insert":
                    var insertCriteria = new InsertSubCompanyCriteria
                    {
                        CompanyId = sub.CompanyId,
                        Name = sub.Name,
                        CreatedBy = session.UserId
                    };
                    commandResult = await baseService.ExecuteNonQueryAsync(insertCriteria);
                    break;

                case "update":
                    var updateCriteria = new UpdateSubCompanyCriteria
                    {
                        SubCompanyId = sub.SubCompanyId,
                        CompanyId = sub.CompanyId,
                        Name = sub.Name,
                        ModifiedBy = session.UserId
                    };
                    commandResult = await baseService.ExecuteNonQueryAsync(updateCriteria);
                    break;

                case "delete":
                    var deleteCriteria = new DeleteSubCompanyCriteria
                    {
                        SubCompanyId = sub.SubCompanyId,
                        DeletedBy = session.UserId
                    };
                    commandResult = await baseService.ExecuteNonQueryAsync(deleteCriteria);
                    break;

                default:
                    continue;
            }
        }

        return Json(commandResult);
    }
}