namespace HRM.Controllers;

using System.Linq;
using System.Text.Json;
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
        var domains = await companyService.GetAllDomainAsync();

        foreach (var company in companies)
        {
            company.SubCompanyList = subCompanies
                .Where(sub => sub.CompanyId == company.CompanyId)
                .ToList();

            company.Urls = JsonSerializer.Serialize(
                domains.Where(d => d.CompanyId == company.CompanyId && !string.IsNullOrWhiteSpace(d.Domain))
                    .Select(s => s.Domain)
                    .Distinct()
            );
        }

        return Json(companies);
    }

    public async Task<IActionResult> GetCurrency()
    {
        var currencies = await companyService.GetCurrencyAsync();
        return Json(currencies);
    }

    public async Task<IActionResult> GetCompanyById(short id)
    {
        var company = await companyService.GetByIdAsync(id);
        var subCompanies = await companyService.GetSubCompanies(id);

        foreach (var sub in subCompanies)
        {
            var countItem = company.SubCompanyEmployees.FirstOrDefault(x => x.SubCompanyId == sub.SubCompanyId && x.CompanyId == sub.CompanyId);
            sub.EmployeeCount = countItem?.EmployeeCount ?? 0;
        }

        var model = new EditCompanySettingViewModel
        {
            CompanyId = company.CompanyId,
            Name = company.Name,
            HasBIO = company.HasBIO,
            WhitelistIPs = company.WhitelistIPs,
            Currency = company.Currency,
            EnableLetstalk = company.EnableLetstalk,
            SubCompanies = subCompanies.ToList()
        };

        return Json(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.CompanyManagement)]
    public ActionResult Edit(int id)
    {
        return View(id);
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

        var criteria = new UpdateSystemConfigsCriteria
        {
            CompanyId = model.CompanyId,
            SettingsJson = model.SettingsJson,
            ModifiedBy = EmployeeSession.EmpId
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
                        CreatedBy = EmployeeSession.EmpId
                    };
                    commandResult = await baseService.ExecuteNonQueryAsync(insertCriteria);
                    break;

                case "update":
                    var updateCriteria = new UpdateSubCompanyCriteria
                    {
                        SubCompanyId = sub.SubCompanyId,
                        CompanyId = sub.CompanyId,
                        Name = sub.Name,
                        ModifiedBy = EmployeeSession.EmpId
                    };
                    commandResult = await baseService.ExecuteNonQueryAsync(updateCriteria);
                    break;

                case "delete":
                    var deleteCriteria = new DeleteSubCompanyCriteria
                    {
                        SubCompanyId = sub.SubCompanyId,
                        DeletedBy = EmployeeSession.EmpId
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