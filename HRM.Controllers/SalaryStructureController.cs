namespace HRM.Controllers;

using System.Threading.Tasks;
using Criteria.SalaryStructure;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Models;
using HRM.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using ViewModels.SalaryStructure;

[HrmPermission(Policy.Read, FunctionKey.SalaryStructureConfiguration)]
public class SalaryStructureController(
    IUserControlSession session,
    IJobLevelService jobLevelService,
    ISalaryStructureService salaryStructureService,
    IBaseService baseService) : BaseController(session)
{
    public ActionResult Index()
    {
        var hasPermission = session.HasPermissionOnFunction(FunctionKey.SalaryStructureConfiguration, Policy.Host);
        return View(hasPermission);
    }

    [HttpPost]
    public async Task<IActionResult> GetSalaryStructures()
    {
        var salaryStructures = await salaryStructureService.GetSalaryStructureAsync(CurrentCompanyId);
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

        var criteria = new InsertSalaryStructureCriteria
        {
            Name = model.Name,
            JobLevelId = model.JobLevelId,
            LocalSalary = model.LocalSalary,
            ExpatSalary = model.ExpatSalary,
            CreatedBy = session.UserId,
            CompanyId = CurrentCompanyId
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
            JobLevelId = salaryStructure.JobLevelId,
            LocalSalary = salaryStructure.LocalSalary,
            ExpatSalary = salaryStructure.ExpatSalary
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

        var criteria = new UpdateSalaryStructureCriteria
        {
            SalaryStructureId = model.SalaryStructureId,
            JobLevelId = model.JobLevelId,
            Name = model.Name,
            LocalSalary = model.LocalSalary,
            ExpatSalary = model.ExpatSalary,
            ModifiedBy = session.UserId,
            CompanyId = CurrentCompanyId
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new JsonResult(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.SalaryStructureConfiguration)]
    public async Task<IActionResult> Delete([FromBody] DeleteSalaryStructureViewModel model)
    {
        var commandResult = await salaryStructureService.DeleteAsync(session.UserId, model.Id);

        return new JsonResult(commandResult);
    }
}