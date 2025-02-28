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
    public IActionResult GetDepartments([FromBody] GetDepartmentViewModel request)
    {
        var departments = departmentService.GetDepartment(request.IsActive);
        return new CustomJsonResult(departments);
    }

    public IActionResult GetUnits()
    {
        var units = unitService.GetUnit();
        return new CustomJsonResult(units);
    }

    [HrmPermission(Policy.Host, FunctionKey.DepartmentManagement)]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.DepartmentManagement)]
    public IActionResult Create([FromBody] CreateDepartmentViewModel model)
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
        commandResult = baseService.ExecuteNonQuery(criteria);

        return new CustomJsonResult(commandResult);
    }

    [HrmPermission(Policy.Host, FunctionKey.DepartmentManagement)]
    public IActionResult GetById(int Id)
    {
        var department = departmentService.GetById(Id);
        var units = unitService.GetUnit();
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
    public IActionResult Edit([FromBody] EditDepartmentViewModel model)
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
        commandResult = baseService.ExecuteNonQuery(criteria);

        return new CustomJsonResult(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.DepartmentManagement)]
    public IActionResult Delete([FromBody] DeleteDepartmentViewModel request)
    {
        var commandResult = departmentService.Delete(EmployeeSession.EmpId, request.Id);

        return new CustomJsonResult(commandResult);
    }
}