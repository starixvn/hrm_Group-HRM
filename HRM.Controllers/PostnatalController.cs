using System;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Models;
using HRM.Criteria.Postnatal;
using HRM.Services.Interfaces;
using HRM.ViewModels.Postnatal;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class PostnatalController(
    IUserControlSession userControlSession,
    IPostnatalService postnatalService) : BaseController(userControlSession)
{
    private readonly IUserControlSession userControlSession = userControlSession;

    [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
    public IActionResult Index()
    {
        ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
        return View();
    }

    [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
    public async Task<PartialViewResult> GetPostnatalGrid(int year)
    {
        var postnatalList = await postnatalService.GetAll(year, CurrentCompanyId);
        ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
        return PartialView("_PostnatalGrid", postnatalList);
    }

    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public PartialViewResult Create()
    {
        var currentDateTime = DateTimeOffset.Now.DateTime.ToClientTimeZone();
        var model = new PostnatalViewModel
        {
            FromDate = currentDateTime,
            ToDate = currentDateTime
        };
        return PartialView("_Create", model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<JsonResult> Create(PostnatalViewModel model)
    {
        var result = new CommandResult();

        if (model != null && ModelState.IsValid)
        {
            var criteria = new InsertPostnatalCriteria
            {
                EmpId = model.EmployeeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                CreatedBy = EmployeeSession.EmpId,
                Remark = model.Remark,
                WorkingHours = model.WorkingHours ?? 0
            };

            result = await postnatalService.Insert(criteria);
        }
        else
        {
            result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
        }

        return Json(result);
    }

    [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
    public async Task<PartialViewResult> Edit(int id)
    {
        var postnatal = await postnatalService.GetById(id);
        var model = new PostnatalViewModel
        {
            EmployeeId = postnatal.EmpId,
            Employee = $"{postnatal.Nickname} ({postnatal.Fullname})",
            FromDate = postnatal.FromDate,
            ToDate = postnatal.ToDate,
            Remark = postnatal.Remark,
            WorkingHours = postnatal.WorkingHours
        };
        return PartialView("_Edit", model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<JsonResult> Update(int id, PostnatalViewModel model)
    {
        var result = new CommandResult();

        if (model != null && ModelState.IsValid)
        {
            var criteria = new UpdatePostnatalCriteria
            {
                PostnatalId = id,
                EmpId = model.EmployeeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                ModifiedBy = EmployeeSession.EmpId,
                Remark = model.Remark,
                WorkingHours = model.WorkingHours ?? 0
            };

            result = await postnatalService.Update(criteria);
        }
        else
        {
            result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any()).Errors.FirstOrDefault().ErrorMessage;
        }

        return Json(result);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<JsonResult> Delete(int id)
    {
        var result = await postnatalService.Delete(id);

        return Json(result);
    }
}