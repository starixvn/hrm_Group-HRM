using System;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Models;
using HRM.Criteria.Employee;
using HRM.Models.Message;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace HRM.Controllers;

public class MessageController(
    IUserControlSession userControlSession,
    IMessageService messageService,
    ICommonDataService commonDataService,
    IEmployeeService employeeService) : BaseController(userControlSession)
{
    private readonly IUserControlSession userControlSession = userControlSession;

    [HrmPermission(Policy.Read, FunctionKey.Message)]
    public async Task<ActionResult> Index()
    {
        var currentDateTime = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        var param = new GetMessageByCreatorParam
        {
            ToDate = currentDateTime,
            FromDate = currentDateTime.AddMonths(-3),
            IsDisabled = 0,
            CreatedBy = EmployeeSession.EmpId,
            CompanyId = CurrentCompanyId
        };

        ViewBag.Messages = await messageService.GetMessageByCreator(param);
        ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.Message, Policy.Host);

        return View(param);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.Message)]
    public async Task<ActionResult> Index(GetMessageByCreatorParam param)
    {
        if (param != null)
        {
            param.CreatedBy = EmployeeSession.EmpId;
        }

        ViewBag.Messages = await messageService.GetMessageByCreator(param);
        ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.Message, Policy.Host);

        return View(param);
    }

    [HrmPermission(Policy.Host, FunctionKey.Message)]
    public async Task<ActionResult> Create()
    {
        var criteria = new GetAllEmployeeCriteria
        {
            Query = null,
            TeamIds = null,
            EnableUser = true,
            IsDeleteUser = null,
            TitleId = null,
            IsLocalUser = null,
            CompanyId = CurrentCompanyId
        };
        ViewBag.Employees = await employeeService.GetEmployeeAsync(criteria);
        ViewBag.Groups = await commonDataService.GetAllTeamAsync();
        var currentDateTime = DateTimeOffset.Now.DateTime.ToClientTimeZone();
        var model = new Message { FromDate = currentDateTime, ToDate = currentDateTime };

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.Message)]
    public async Task<JsonResult> Create(Message model)
    {
        var commandResult = new CommandResult();

        if (model != null && ModelState.IsValid)
        {
            commandResult = await messageService.Create(EmployeeSession.EmpId, model, CurrentCompanyId);
            commandResult.CompanyId = CurrentCompanyId;
            if (model.HasSendEmail && commandResult.IsSuccessful)
            {
                await messageService.SendMailToStaff(model);
            }
        }

        return new JsonResult(commandResult, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
    }

    [HrmPermission(Policy.Host, FunctionKey.Message)]
    public async Task<ActionResult> Edit(int id)
    {
        ViewBag.Message = await messageService.GetMessageById(id);
        var criteria = new GetAllEmployeeCriteria
        {
            Query = null,
            TeamIds = null,
            EnableUser = true,
            IsDeleteUser = false,
            TitleId = null,
            IsLocalUser = null,
            CompanyId = CurrentCompanyId
        };

        ViewBag.Employees = await employeeService.GetEmployeeAsync(criteria);
        ViewBag.Groups = await commonDataService.GetAllTeamAsync();

        return View();
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.Message)]
    public async Task<JsonResult> Edit(Message model)
    {
        var commandResult = new CommandResult();

        if (model != null && ModelState.IsValid)
        {
            commandResult = await messageService.Edit(EmployeeSession.EmpId, model);
            if (model.HasSendEmail && commandResult.IsSuccessful)
            {
                await messageService.SendMailToStaff(model);
            }
        }

        return new JsonResult(commandResult, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.Message)]
    public async Task<JsonResult> Delete(int id)
    {
        var commandResult = await messageService.Delete(EmployeeSession.EmpId, id);

        return new JsonResult(commandResult, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
    }

    public async Task<ActionResult> ViewMessageOfEmployee()
    {
        var currentDateTime = DateTimeOffset.Now.DateTime.ToClientTimeZone();
        var param = new GetMessageByEmployeeParam
        {
            FromDate = currentDateTime,
            ToDate = currentDateTime,
            IsDisabled = 0,
            EmployeeId = EmployeeSession.EmpId
        };
        ViewBag.Messages = await messageService.GetMessageByEmployee(param);

        return View(param);
    }

    [HttpPost]
    public async Task<ActionResult> ViewMessageOfEmployee(GetMessageByEmployeeParam param)
    {
        if (param != null)
        {
            param.EmployeeId = EmployeeSession.EmpId;
        }

        ViewBag.Messages = await messageService.GetMessageByEmployee(param);

        return View(param);
    }

    [HttpPost]
    public async Task<JsonResult> DeleteMessageByEmployee(int id)
    {
        var commandResult = await messageService.DeleteMessageByEmployee(EmployeeSession.EmpId, id);

        return Json(commandResult);
    }

    [HrmPermission(Policy.Read, FunctionKey.Message)]
    public async Task<ActionResult> Detail(int id)
    {
        var message = await messageService.GetMessageById(id);
        return View(message);
    }

    public async Task<ActionResult> DetailByEmployee(int id)
    {
        var message = await messageService.GetDetailMessageByEmployee(id);
        return View(message);
    }

    public async Task<JsonResult> EditMessageStatus(int id)
    {
        var commandResult = await messageService.EditStatus(EmployeeSession.EmpId, id);

        return Json(commandResult);
    }
}