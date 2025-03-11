namespace HRM.Controllers;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Constants;
using Core.Filters;
using Core.Models;
using Core.Services;
using Criteria.ExtensionNumber;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Services;
using Services.Implement;
using ViewModels.ExtensionNumber;

public class ExtensionNoController(
    IUserControlSession userControlSession,
    IExtensionNumberService extensionNumberService,
    ICommonDataService commonDataService,
    IBaseService baseService) : BaseController(userControlSession)
{
    [HrmPermission(Policy.Read, FunctionKey.ExtensionNumber)]
    public IActionResult Index()
    {
        var hasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.ExtensionNumber, Policy.Host);

        return View(hasPermission);
    }

    [HrmPermission(Policy.Read, FunctionKey.ExtensionNumber)]
    public async Task<IActionResult> GetExtensionNo()
    {
        var extensionNos = await extensionNumberService.GetAllExtensionNoAsync(CurrentCompanyId);

        var model = extensionNos.GroupBy(item => item.TeamName)
            .Select(item => new ExtensionNumberViewModel { Name = item.Key, Extensions = item.ToList() }).ToList();
        return Json(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.ExtensionNumber)]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ExtensionNumber)]
    public async Task<IActionResult> Create([FromBody] ExtensionNoModel model)
    {
        var commandResult = new CommandResult();
        CheckExternalEmployee(model);
        if (ModelState.IsValid)
        {
            var criteria = new InsertExtensionNumberCriteria
            {
                FullName = model.FullName,
                NickName = model.NickName,
                ExtensionNo = model.ExtensionNo,
                IsInternal = model.IsInternal,
                EmpId = model.EmpId,
                TeamId = model.TeamId,
            };

            commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        }

        return Json(commandResult);
    }

    [HrmPermission(Policy.Host, FunctionKey.ExtensionNumber)]
    public async Task<IActionResult> GetExtentionById(int id, bool isInternal)
    {
        var model = await extensionNumberService.GetEmployeeExtensionNoByIdAsync(id, isInternal);
        return Json(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.ExtensionNumber)]
    public IActionResult Edit(int id, bool isInternal)
    {
        ViewBag.Id = id;
        ViewBag.IsInternal = isInternal;

        return View();
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ExtensionNumber)]
    public async Task<IActionResult> Edit([FromBody] ExtensionNoModel model)
    {
        var commandResult = new CommandResult();
        CheckExternalEmployee(model);
        if (ModelState.IsValid)
        {
            var criteria = new UpdateExtensionNumberMemberCriteria
            {
                FullName = model.FullName,
                NickName = model.NickName,
                ExtensionNo = model.ExtensionNo,
                IsInternal = model.IsInternal,
                ExtId = model.EmployeeExtensionId,
                EmpId = model.EmpId,
                TeamId = model.TeamId
            };
            commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        }

        return Json(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ExtensionNumber)]
    public async Task<IActionResult> Delete(int id)
    {
        var commandResult = await extensionNumberService.DeleteExtensionNumberAsync(id);
        return Json(commandResult);
    }

    #region External Group

    [HrmPermission(Policy.Host, FunctionKey.ExtensionNumber)]
    public IActionResult ExternalGroup()
    {
        return View();
    }

    [HrmPermission(Policy.Host, FunctionKey.ExtensionNumber)]
    public async Task<IActionResult> GetAllExternalGroup()
    {
        var groups = await extensionNumberService.GetAllExtensionGroupInfoAsync( CurrentCompanyId, isInternal: false);

        return Json(groups);
    }

    [HttpGet]
    [HrmPermission(Policy.Host, FunctionKey.ExtensionNumber)]
    public async Task<IActionResult> GetGroupExtension(bool isInternal, int? teamId)
    {
        IList<SelectListItem> groups;
        if (isInternal)
        {
            var teams = await commonDataService.GetAllTeamAsync();
            groups = teams.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }
        else
        {
            var extensionGroups = await extensionNumberService.GetAllExtensionGroupInfoAsync(CurrentCompanyId, teamId, isInternal);
            groups = extensionGroups.Select(x => new SelectListItem
            {
                Text = x.TeamName,
                Value = x.TeamId.ToString()
            }).ToList();
        }

        return Json(groups);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ExtensionNumber)]
    public async Task<IActionResult> CreateExternalGroup([FromBody] CreateExternalGroupCriteria criteria)
    {
        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        return Json(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ExtensionNumber)]
    public async Task<IActionResult> DeleteExternalGroup(int id)
    {
        var commandResult = await extensionNumberService.DeleteExternalGroupAsync(id);
        return Json(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ExtensionNumber)]
    public async Task<IActionResult> EditExternalGroup([FromBody] UpdateExternalNumberCriteria criteria)
    {
        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        return Json(commandResult);
    }

    #endregion External Group

    private void CheckExternalEmployee(ExtensionNoModel model)
    {
        if (!model.IsInternal)
        {
            var propertyNames = ModelState.Select(x => x.Key).Where(x => x.Contains("EmpId")).ToList();
            foreach (var propertyName in propertyNames)
            {
                ModelState[propertyName].Errors.Clear();
            }
        }
    }
}