namespace HRM.Controllers;

using System.Threading.Tasks;
using Core.Constants;
using Core.Filters;
using Criteria.Department;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Employee;
using HRM.Models;
using HRM.ViewModels.CompanySecondmentEmployee;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using ViewModels.Department;

public class SecondmentCompanyController(
    IUserControlSession session,
    ISecondCompanyEmployeeService secondCompanyEmployeeService) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.SecondmentCompanyList)]
    public ActionResult Index()
    {
        var hasPermission = session.HasPermissionOnFunction(FunctionKey.SecondmentCompanyList, Policy.Host);
        return View(hasPermission);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.SecondmentCompanyList)]
    public async Task<IActionResult> GetSecondmentCompanyList([FromBody] GetCompanySecondEmployeeViewModel request)
    {
        var secondmentCompanyList = await secondCompanyEmployeeService.GetAllAsync();
        return new JsonResult(secondmentCompanyList);
    }

    [HrmPermission(Policy.Host, FunctionKey.SecondmentCompanyList)]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.SecondmentCompanyList)]
    public async Task<IActionResult> Create([FromBody] CreateCompanySecondEmployeeViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid)
        {
            return new JsonResult(commandResult);
        }

        commandResult = await secondCompanyEmployeeService.CreateAsync(session.UserId, model.EmpId, model.CompanyId);

        return new JsonResult(commandResult);
    }

    [HrmPermission(Policy.Host, FunctionKey.SecondmentCompanyList)]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await secondCompanyEmployeeService.GetByIdAsync(id);

        return new JsonResult(employee);
    }

    [HrmPermission(Policy.Host, FunctionKey.SecondmentCompanyList)]
    public ActionResult Edit(int id)
    {
        return View(id);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.SecondmentCompanyList)]
    public async Task<IActionResult> Edit([FromBody] UpdateCompanySecondEmployeeViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid)
        {
            return new JsonResult(commandResult);
        }
        commandResult = await secondCompanyEmployeeService.UpdateAsync(session.UserId, model.OldEmpId, model.EmpId, model.CompanyId);

        return new JsonResult(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.SecondmentCompanyList)]
    public async Task<IActionResult> Delete([FromBody] DeleteCompanySecondEmployeeViewModel request)
    {
        var commandResult = await secondCompanyEmployeeService.DeleteAsync(request.EmpId);

        return new JsonResult(commandResult);
    }
}