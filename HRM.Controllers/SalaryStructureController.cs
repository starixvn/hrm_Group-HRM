namespace HRM.Controllers;

using System.Threading.Tasks;
using Criteria.SalaryStructure;
using Fanex.Data;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Company;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.Interfaces;
using ViewModels.SalaryStructure;

[HrmPermission(Policy.Read, FunctionKey.SalaryStructureConfiguration)]
public class SalaryStructureController(
    IUserControlSession session,
    IJobLevelService jobLevelService,
    ISalaryStructureService salaryStructureService,
    IBaseService baseService
) : BaseController(session)
{
    public ActionResult Index()
    {
        var hasPermission = session.HasPermissionOnFunction(
            FunctionKey.SalaryStructureConfiguration,
            Policy.Host
        );
        return View(hasPermission);
    }

    [HttpPost]
    public async Task<IActionResult> GetSalaryStructures()
    {
        var salaryStructures = await salaryStructureService.GetSalaryStructureAsync(
            CurrentCompanyId
        );
        return Json(salaryStructures);
    }

    public async Task<IActionResult> GetJobLevels()
    {
        var jobLevels = await jobLevelService.GetJobLevel();
        return Json(jobLevels);
    }

    [HrmPermission(Policy.Host, FunctionKey.SalaryStructureConfiguration)]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.SalaryStructureConfiguration)]
    public async Task<IActionResult> Create([FromBody] CreateSalaryStructureViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid)
        {
            return new JsonResult(commandResult);
        }

        foreach (var item in model.Details)
        {
            item.BasicSalary = !string.IsNullOrWhiteSpace(item.BasicSalary)
                ? Cryptography.EncryptPassword(item.BasicSalary)
                : null;
            item.ExpatSalary = !string.IsNullOrWhiteSpace(item.ExpatSalary)
                ? Cryptography.EncryptPassword(item.ExpatSalary)
                : null;
            item.JobLevelAddOn = !string.IsNullOrWhiteSpace(item.JobLevelAddOn)
                ? Cryptography.EncryptPassword(item.JobLevelAddOn)
                : null;
        }

        var criteria = new InsertSalaryStructureCriteria
        {
            CompanyId = CurrentCompanyId,
            Name = model.Name,
            Details = JsonConvert.SerializeObject(model.Details),
            CreatedBy = EmployeeSession.EmpId,
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new JsonResult(commandResult);
    }

    public async Task<IActionResult> GetById(int Id)
    {
        var salaryStructure = await salaryStructureService.GetByIdAsync(Id);

        var model = new EditSalaryStructureViewModel
        {
            SalaryStructureId = salaryStructure.SalaryStructureId,
            Name = salaryStructure.Name,
            Details = salaryStructure.Details,
        };

        return new JsonResult(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.SalaryStructureConfiguration)]
    public ActionResult Edit(int id)
    {
        return View(id);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.SalaryStructureConfiguration)]
    public async Task<IActionResult> Edit([FromBody] EditSalaryStructureViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid)
        {
            return new JsonResult(commandResult);
        }

        foreach (var item in model.Details)
        {
            item.BasicSalary = !string.IsNullOrWhiteSpace(item.BasicSalary)
                ? Cryptography.EncryptPassword(item.BasicSalary)
                : null;
            item.ExpatSalary = !string.IsNullOrWhiteSpace(item.ExpatSalary)
                ? Cryptography.EncryptPassword(item.ExpatSalary)
                : null;
            item.JobLevelAddOn = !string.IsNullOrWhiteSpace(item.JobLevelAddOn)
                ? Cryptography.EncryptPassword(item.JobLevelAddOn)
                : null;
        }

        var criteria = new UpdateSalaryStructureCriteria
        {
            CompanyId = CurrentCompanyId,
            SalaryStructureId = model.SalaryStructureId,
            Name = model.Name,
            Details = JsonConvert.SerializeObject(model.Details),
            ModifiedBy = EmployeeSession.EmpId,
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new JsonResult(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.SalaryStructureConfiguration)]
    public async Task<IActionResult> Delete([FromBody] DeleteSalaryStructureViewModel model)
    {
        var commandResult = await salaryStructureService.DeleteAsync(
            EmployeeSession.EmpId,
            model.Id
        );

        return new JsonResult(commandResult);
    }

    [HttpGet]
    [HrmPermission(Policy.Host, FunctionKey.SalaryStructureConfiguration)]
    public async Task<IActionResult> GetBasicSalaryConfig()
    {
        var currentConfig = await salaryStructureService.GetBasicSalaryConfigAsync(
            CurrentCompanyId
        );

        return Json(currentConfig);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.SalaryStructureConfiguration)]
    public async Task<IActionResult> EditBasicSalaryConfig([FromBody] object configData)
    {
        var commandResult = new CommandResult();
        if (configData == null)
        {
            return Json(commandResult);
        }

        var settingsJson = JsonConvert.SerializeObject(configData);

        var criteria = new UpdateSystemConfigsCriteria
        {
            CompanyId = CurrentCompanyId,
            SettingsJson = settingsJson,
            ModifiedBy = EmployeeSession.EmpId,
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return Json(commandResult);
    }
}
