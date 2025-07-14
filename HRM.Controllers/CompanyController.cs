namespace HRM.Controllers;

using System.Threading.Tasks;
using Criteria.SalaryStructure;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using ViewModels.SalaryStructure;
using HRM.Core.Models;
using HRM.Core.Services;
using Fanex.Data;
using HRM.Models;
using HRM.ViewModels.Company;

public class CompanyController(
    IUserControlSession session,
    ICompanyService companyService,
    IBaseService baseService) : BaseController(session)
{
    public ActionResult Index()
    {
        return View();
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

    public ActionResult Edit(int Id)
    {
        return View(Id);
    }

    //[HttpPost]
    //public async Task<IActionResult> Edit([FromBody] EditCompanySettingViewModel model)
    //{
    //    var commandResult = new CommandResult();
    //    if (model == null || !ModelState.IsValid)
    //    {
    //        return new JsonResult(commandResult);
    //    }

    //    var criteria = new UpdateSalaryStructureCriteria
    //    {
    //        SalaryStructureId = model.SalaryStructureId,
    //        JobLevelId = model.JobLevelId,
    //        Name = model.Name,
    //        ModifiedBy = session.UserId,
    //        CompanyId = CurrentCompanyId
    //    };
    //    commandResult = await baseService.ExecuteNonQueryAsync(criteria);

    //    return new JsonResult(commandResult);
    //}
}