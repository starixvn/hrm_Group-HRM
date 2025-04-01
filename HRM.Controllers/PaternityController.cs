using System;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Models;
using HRM.Criteria.Paternity;
using HRM.Services.Interfaces;
using HRM.ViewModels.Paternity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HRM.Controllers;

public class PaternityController(
    IUserControlSession session,
    IPaternityService paternityService) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
    public ActionResult Index()
    {
        ViewBag.IsHost = session.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
        return View();
    }

    [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
    public async Task<PartialViewResult> ListPaternity(int year)
    {
        var paternityList = await paternityService.GetAll(year, CurrentCompanyId);
        ViewBag.IsHost = session.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
        return PartialView("_ListPaternity", paternityList);
    }

    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public ActionResult Create()
    {
        var model = new PaternityViewModel
        {
            ChildBornDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
            LeaveDay = 5
        };
        return PartialView("_Create", model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<JsonResult> Create(PaternityViewModel model)
    {
        var result = new CommandResult();

        if (model != null && ModelState.IsValid)
        {
            var criteria = new InsertPaternityCriteria
            {
                EmpId = model.EmployeeId,
                ChildBornDate = model.ChildBornDate,
                LeaveDay = model.LeaveDay,
                CreatedBy = session.UserId,
                Remark = model.Remark
            };

            result = await paternityService.Insert(criteria);
        }
        else
        {
            result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
        }

        return new JsonResult(result, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
    }

    [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
    public async Task<PartialViewResult> Edit(int id)
    {
        var paternity = await paternityService.GetById(id);

        var model = new PaternityViewModel
        {
            EmployeeId = paternity.EmpId,
            Employee = $"{paternity.Nickname} ({paternity.Fullname})",
            ChildBornDate = paternity.ChildBornDate,
            LeaveDay = paternity.LeaveDay,
            Remark = paternity.Remark
        };
        return PartialView("_Edit", model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<JsonResult> Update(int id, PaternityViewModel model)
    {
        var result = new CommandResult();

        if (model != null && ModelState.IsValid)
        {
            var criteria = new UpdatePaternityCriteria
            {
                PaternityId = id,
                EmpId = model.EmployeeId,
                ChildBornDate = model.ChildBornDate,
                LeaveDay = model.LeaveDay,
                ModifiedBy = session.UserId,
                Remark = model.Remark
            };

            result = await paternityService.Update(criteria);
        }
        else
        {
            result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
        }

        return new JsonResult(result, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<JsonResult> Delete(int id)
    {
        var result = await paternityService.Delete(id);

        return new JsonResult(result, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
    }
}