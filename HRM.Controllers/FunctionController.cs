using GO.UserControl.Client.Core.Models;
using GO.UserControl.Client.Core.Services;
using System.Collections.Generic;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core;
using HRM.ViewModels.Function;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class FunctionController(IUserControlSession userControlSession,
    IRoleService roleService,
    IFunctionService functionService) : BaseController(userControlSession)
{
    [HrmPermission(Policy.Read, FunctionKey.FunctionManagement)]
    public ActionResult Index()
    {
        return View();
    }

    [HrmPermission(Policy.Read, FunctionKey.FunctionManagement)]
    public IActionResult GetRolePermission([FromBody] GetRolePermissionViewModel request)
    {
        var systemRoles = roleService.GetBySiteId(userControlSession.SiteId);
        var functionTrees = new TreeFunctionViewModel
        {
            RoleId = request.RoleId ?? systemRoles[0].RoleId,
            IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.FunctionManagement, Policy.Host),
            Functions = functionService.GetBySiteId(userControlSession.SiteId),
            HierarchyFunctions = functionService.GetHierarchyBySiteId(userControlSession.SiteId),
            Permissions = functionService.GetPermissionBySiteAndRoleId(userControlSession.SiteId, request.RoleId ?? systemRoles[0].RoleId)
        };

        return new CustomJsonResult(functionTrees);
    }

    [HrmPermission(Policy.Host, FunctionKey.FunctionManagement)]
    public IActionResult GetSystemRoles()
    {
        var systemRoles = roleService.GetBySiteId(userControlSession.SiteId);
        return new CustomJsonResult(systemRoles);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.FunctionManagement)]
    public CustomJsonResult AssignPermission([FromBody] TreeFunctionViewModel request)
    {
        var commandResult = roleService.AssignPermission(userControlSession.SiteId, request.RoleId, request.Permissions);
        return new CustomJsonResult(commandResult);
    }
}