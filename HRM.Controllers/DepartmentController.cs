using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Department;
using HRM.Services;
using HRM.Services.Interfaces;
using HRM.ViewModels.Department;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class DepartmentController(IUserControlSession userControlSession,
    IDepartmentService departmentService,
    IUnitService unitService,
    IBaseService baseService) : BaseController(userControlSession)
{
    [HrmPermission(Policy.Read, FunctionKey.DepartmentManagement)]
    public ActionResult Index()
    {
        var hasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.DepartmentManagement, Policy.Host);
        return View(hasPermission);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.DepartmentManagement)]
    public async Task<IActionResult> GetDepartments([FromBody] GetDepartmentViewModel request)
    {
        var departments = await departmentService.GetDepartmentAsync(request.IsActive);
        return new CustomJsonResult(departments);
    }

    public async Task<IActionResult> GetUnits()
    {
        var units = await unitService.GetUnitAsync();
        return new CustomJsonResult(units);
    }

    [HrmPermission(Policy.Host, FunctionKey.DepartmentManagement)]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.DepartmentManagement)]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid) return new CustomJsonResult(commandResult);

        var criteria = new InsertDepartmentCriteria
        {
            Code = model.Code,
            Name = model.Name,
            IsActive = model.IsActive,
            UnitId = model.UnitId,
            ModifiedBy = EmployeeSession.EmpId
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new CustomJsonResult(commandResult);
    }

    [HrmPermission(Policy.Host, FunctionKey.DepartmentManagement)]
    public async Task<IActionResult> GetById(int Id)
    {
        var department = await departmentService.GetByIdAsync(Id);
        var units = await unitService.GetUnitAsync();
        var model = new EditDepartmentViewModel
        {
            DepartmentId = Id,
            Code = department.Code,
            Name = department.Name,
            IsActive = department.IsActive,
            UnitId = department.Unit?.Id ?? 0,
            Units = units
        };

        return new CustomJsonResult(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.DepartmentManagement)]
    public ActionResult Edit(int id)
    {
        return View(id);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.DepartmentManagement)]
    public async Task<IActionResult> Edit([FromBody] EditDepartmentViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid) return new CustomJsonResult(commandResult);

        var criteria = new UpdateDepartmentCriteria
        {
            DepartmentId = model.DepartmentId,
            Code = model.Code,
            Name = model.Name,
            IsActive = model.IsActive,
            UnitId = model.UnitId,
            ModifiedBy = EmployeeSession.EmpId
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new CustomJsonResult(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.DepartmentManagement)]
    public async Task<IActionResult> Delete([FromBody] DeleteDepartmentViewModel request)
    {
        var commandResult = await departmentService.DeleteAsync(EmployeeSession.EmpId, request.Id);

        return new CustomJsonResult(commandResult);
    }
}