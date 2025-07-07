
namespace HRM.Controllers;

using System.Threading.Tasks;
using Criteria.SalaryStructure;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.ViewModels.Department;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using ViewModels.SalaryStructure;

public class SalaryStructureController(
    IUserControlSession session,
    IJobLevelService jobLevelService,
    ISalaryStructureService salaryStructureService,
    IBaseService baseService) : BaseController(session)
{
    //  [HrmPermission(Policy.Read, FunctionKey.SalaryStructureManagement)]
    public ActionResult Index()
    {
        var hasPermission = true; //session.HasPermissionOnFunction(FunctionKey.SalaryStructureManagement, Policy.Host);
        return View(hasPermission);
    }

    [HttpPost]
    // [HrmPermission(Policy.Read, FunctionKey.SalaryStructureManagement)]
    public async Task<IActionResult> GetSalaryStructures()
    {
        var salaryStructures = await salaryStructureService.GetSalaryStructureAsync(CurrentCompanyId);
        return new JsonResult(salaryStructures);
    }

    public async Task<IActionResult> GetJobLevels()
    {
        var jobLevels = await jobLevelService.GetJobLevel();
        return new JsonResult(jobLevels);
    }

     //[HrmPermission(Policy.Host, FunctionKey.SalaryStructureManagement)]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    // [HrmPermission(Policy.Host, FunctionKey.SalaryStructureManagement)]
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
            CreatedBy = session.UserId
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new JsonResult(commandResult);
    }

    //[HrmPermission(Policy.Host, FunctionKey.SalaryStructureManagement)]
    public async Task<IActionResult> GetById(int Id)
    {
        var salaryStructure = await salaryStructureService.GetByIdAsync(Id);

        var model = new EditSalaryStructureViewModel
        {
            SalaryStructureId = Id,
            Name = salaryStructure.Name,
            LocalSalary = salaryStructure.LocalSalary,
            ExpatSalary = salaryStructure.ExpatSalary
        };

        return new JsonResult(model);
    }

    // [HrmPermission(Policy.Host, FunctionKey.SalaryStructureManagement)]
    public ActionResult Edit(int id)
    {
        return View(id);
    }

    [HttpPost]
    // [HrmPermission(Policy.Host, FunctionKey.SalaryStructureManagement)]
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
    // [HrmPermission(Policy.Host, FunctionKey.SalaryStructureManagement)]
    public async Task<IActionResult> Delete([FromBody] DeleteSalaryStructureViewModel model)
    {
        var commandResult = await salaryStructureService.DeleteAsync(session.UserId, model.Id);

        return new JsonResult(commandResult);
    }
}
