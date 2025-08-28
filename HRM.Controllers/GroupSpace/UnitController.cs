using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Models;
using HRM.Criteria.Employee;
using HRM.Criteria.Unit;
using HRM.Services.Interfaces;
using HRM.ViewModels.Unit;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.GroupSpace;

public class UnitController(
    IUserControlSession session,
    IEmployeeService employeeService,
    IUnitService unitService) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.UnitManagement)]
    public ActionResult Index()
    {
        var hasPermission = session.HasPermissionOnFunction(FunctionKey.UnitManagement, Policy.Host);
        return View(hasPermission);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.UnitManagement)]
    public async Task<IActionResult> GetUnits([FromBody] UnitViewModel model)
    {
        var units = await unitService.GetUnitAsync(model.IsActive);

        return new JsonResult(units);
    }

    [HrmPermission(Policy.Host, FunctionKey.UnitManagement)]
    public async Task<IActionResult> Create()
    {
        ViewBag.NextId = await unitService.GetNextGroupUnitIdAsync();
        return View();
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.UnitManagement)]
    public async Task<IActionResult> Create([FromBody] AddUnitViewModel model)
    {
        var commandResult = new CommandResult();

        if (model == null || !ModelState.IsValid)
        {
            return new JsonResult(commandResult);
        }

        var criteria = new InsertUnitCriteria
        {
            Code = model.Code,
            Name = model.Code,
            GroupUnitId = model.GroupUnitId,
            PICEmpId = model.PICEmpId,
            Category = model.Category,
            Type = model.Type,
            AccountableArea = model.AccountableArea,
            IsActive = model.IsActive,
            CreatedBy = EmployeeSession.EmpId
        };
        commandResult = await unitService.CreateAsync(criteria);
        return new JsonResult(commandResult);
    }

    [HrmPermission(Policy.Host, FunctionKey.UnitManagement)]
    public async Task<IActionResult> GetById(int id)
    {
        var unit = await unitService.GetByIdAsync(id);
        return new JsonResult(unit);
    }

    [HrmPermission(Policy.Host, FunctionKey.UnitManagement)]
    public ActionResult Edit(int id)
    {
        return View(id);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.UnitManagement)]
    public async Task<IActionResult> Edit([FromBody] EditUnitViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid)
        {
            return new JsonResult(commandResult);
        }

        var criteria = new UpdateUnitCriteria
        {
            UnitId = model.UnitId,
            Code = model.Code,
            Name = model.Code,
            GroupUnitId = model.GroupUnitId,
            PICEmpId = model.PICEmpId,
            Category = model.Category,
            Type = model.Type,
            AccountableArea = model.AccountableArea,
            IsActive = model.IsActive,
            ModifiedBy = EmployeeSession.EmpId
        };
        commandResult = await unitService.EditAsync(criteria);
        return new JsonResult(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.UnitManagement)]
    public async Task<IActionResult> Delete([FromBody] DeleteUnitCriteria criteria)
    {
        criteria.ModifiedBy = EmployeeSession.EmpId;
        var commandResult = await unitService.DeleteAsync(criteria);

        return new JsonResult(commandResult);
    }

    [HttpGet]
    [HrmPermission(Policy.Host, FunctionKey.UnitManagement)]
    public async Task<JsonResult> GetEmployeeForPIC()
    {
        var employees = await employeeService.GetEmployeeForPICAsync(new GetEmployeeForPICCriteria());
        return new JsonResult(employees);
    }
}