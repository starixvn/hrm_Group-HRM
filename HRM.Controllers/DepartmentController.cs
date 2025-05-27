
namespace HRM.Controllers;

using System;
using System.Threading.Tasks;
using Core.Constants;
using Core;
using Core.Filters;
using Criteria.Department;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Models;
using HRM.Core.Services;
using Criteria.Employee;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using ViewModels.Department;

public class DepartmentController(
    IUserControlSession session,
    IDepartmentService departmentService,
    IUnitService unitService,
    IEmployeeService employeeService,
    IBaseService baseService) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.DepartmentManagement)]
    public ActionResult Index()
    {
        var hasPermission = session.HasPermissionOnFunction(FunctionKey.DepartmentManagement, Policy.Host);
        return View(hasPermission);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.DepartmentManagement)]
    public async Task<IActionResult> GetDepartments([FromBody] GetDepartmentViewModel request)
    {
        var departments = await departmentService.GetDepartmentAsync(request.IsActive);
        return new JsonResult(departments);
    }

    public async Task<IActionResult> GetUnits()
    {
        var units = await unitService.GetUnitAsync();
        return new JsonResult(units);
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
        if (model == null || !ModelState.IsValid)
        {
            return new JsonResult(commandResult);
        }

        var criteria = new InsertDepartmentCriteria
        {
            Code = model.Code,
            Name = model.Name,
            GroupDeptId = model.GroupDeptId,
            PICEmpId = model.PICEmpId,
            AccountableArea = model.AccountableArea,
            IsActive = model.IsActive,
            UnitId = model.UnitId,
            ModifiedBy = session.UserId
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new JsonResult(commandResult);
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
            GroupDeptId = department.GroupDeptId,
            PICEmpId = department.PICEmpId,
            AccountableArea = department.AccountableArea,
            IsActive = department.IsActive,
            UnitId = department.Unit?.Id ?? 0,
            Units = units
        };

        return new JsonResult(model);
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
        if (model == null || !ModelState.IsValid)
        {
            return new JsonResult(commandResult);
        }
        var criteria = new UpdateDepartmentCriteria
        {
            DepartmentId = model.DepartmentId,
            Code = model.Code,
            Name = model.Name,
            GroupDeptId = model.GroupDeptId,
            PICEmpId = model.PICEmpId,
            AccountableArea = model.AccountableArea,
            IsActive = model.IsActive,
            UnitId = model.UnitId,
            ModifiedBy = session.UserId
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new JsonResult(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.DepartmentManagement)]
    public async Task<IActionResult> Delete([FromBody] DeleteDepartmentViewModel request)
    {
        var commandResult = await departmentService.DeleteAsync(session.UserId, request.Id);

        return new JsonResult(commandResult);
    }

    [HttpGet]
    [HrmPermission(Policy.Host, FunctionKey.UnitManagement)]
    public async Task<JsonResult> GetPrepareDataForUnit()
    {
        var employees = await employeeService.GetEmployeeForPICAsync(new GetEmployeeForPICCriteria());
        var nextId = await departmentService.GetNextGroupDeptIdAsync();
        return new JsonResult(new { employees, nextId });
    }

    [HrmPermission(Policy.Read, FunctionKey.DepartmentManagement)]
    public async Task<IActionResult> ExportDepartmentList(GetDepartmentViewModel request)
    {
        var departments = await departmentService.GetDepartmentAsync(request.IsActive);
        var bytes = await departmentService.GenerateExcelAsync(departments);
        var fileName = $"Departments_{DateTimeOffset.Now.DateTime.ToClientTimeZone():yyyyMMdd}.xlsx";

        return File(bytes, MimeType.XLS, fileName);
    }
}