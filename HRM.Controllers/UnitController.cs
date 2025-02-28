using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core;
using HRM.Criteria.Unit;
using HRM.Services;
using HRM.ViewModels.Unit;
using Microsoft.AspNetCore.Mvc;
using HRM.Core.Models;
namespace HRM.Controllers;

public class UnitController(IUserControlSession userControlSession,
    IUnitService unitService) : BaseController(userControlSession)
{
    private readonly IUserControlSession userControlSession1 = userControlSession;

    [HrmPermission(Policy.Read, FunctionKey.UnitManagement)]
    public ActionResult Index()
    {
        var hasPermission = userControlSession1.HasPermissionOnFunction(FunctionKey.UnitManagement, Policy.Host);
        return View(hasPermission);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.UnitManagement)]
    public IActionResult GetUnits([FromBody] UnitViewModel model)
    {
        var units = unitService.GetUnit(model.IsActive);
        return new CustomJsonResult(units);
    }

    [HrmPermission(Policy.Host, FunctionKey.UnitManagement)]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.UnitManagement)]
    public IActionResult Create([FromBody] AddUnitViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid) return new CustomJsonResult(commandResult);

        var criteria = new InsertUnitCriteria
        {
            Code = model.Code,
            Name = model.Name,
            IsActive = model.IsActive,
            CreatedBy = EmployeeSession.EmpId
        };
        commandResult = unitService.Create(criteria);
        return new CustomJsonResult(commandResult);
    }

    [HrmPermission(Policy.Host, FunctionKey.UnitManagement)]
    public IActionResult GetById(int id)
    {
        var unit = unitService.GetById(id);
        return new CustomJsonResult(unit);
    }

    [HrmPermission(Policy.Host, FunctionKey.UnitManagement)]
    public ActionResult Edit(int id)
    {
        return View(id);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.UnitManagement)]
    public IActionResult Edit([FromBody] EditUnitViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid) return new CustomJsonResult(commandResult);

        var criteria = new UpdateUnitCriteria
        {
            UnitId = model.UnitId,
            Code = model.Code,
            Name = model.Name,
            IsActive = model.IsActive,
            ModifiedBy = EmployeeSession.EmpId
        };
        commandResult = unitService.Edit(criteria);
        return new CustomJsonResult(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.UnitManagement)]
    public IActionResult Delete([FromBody] DeleteUnitCriteria criteria)
    {
        criteria.ModifiedBy = EmployeeSession.EmpId;
        var commandResult = unitService.Delete(criteria);

        return new CustomJsonResult(commandResult);
    }
}