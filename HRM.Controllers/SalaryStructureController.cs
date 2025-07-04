
namespace HRM.Controllers;

using System.Threading.Tasks;
using Core.Constants;
using Core.Filters;
using Criteria.Department;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Models;
using HRM.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using ViewModels.Department;

public class SalaryStructureController(
    IUserControlSession session,
    IJobLevelService jobLevelService,
    ISalaryStructureService salaryStructureService,
    IEmployeeService employeeService,
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
        var salaryStructures = await salaryStructureService.Get(request.IsActive);
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
    public async Task<IActionResult> Create([FromBody] CreateDepartmentViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid)
        {
            return new JsonResult(commandResult);
        }

        var criteria = new InsertDepartmentCriteria
        {
            Code = model.Code,
            Name = model.Name,
            //ModifiedBy = session.UserId
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new JsonResult(commandResult);
    }

    // [HrmPermission(Policy.Host, FunctionKey.SalaryStructureManagement)]
    public async Task<IActionResult> GetById(int Id)
    {
        var department = await departmentService.GetByIdAsync(Id);
        var units = await unitService.GetUnitAsync();
        var model = new EditDepartmentViewModel
        {
            DepartmentId = Id,
            Code = department.Code,
            Name = department.Name,
            UnitId = department.Unit?.Id ?? 0,
            Units = units
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
    public async Task<IActionResult> Edit([FromBody] EditDepartmentViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid)
        {
            return new JsonResult(commandResult);
        }
        var criteria = new UpdateDepartmentCriteria
        {
            DepartmentId = model.DepartmentId,
            Code = model.Code,
            Name = model.Name,
            ModifiedBy = session.UserId
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new JsonResult(commandResult);
    }

    [HttpPost]
    // [HrmPermission(Policy.Host, FunctionKey.SalaryStructureManagement)]
    public async Task<IActionResult> Delete([FromBody] DeleteDepartmentViewModel request)
    {
        var commandResult = await departmentService.DeleteAsync(session.UserId, request.Id);

        return new JsonResult(commandResult);
    }
}
