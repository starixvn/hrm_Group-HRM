namespace HRM.Controllers;

using System.Threading.Tasks;
using Core.Constants;
using Core.Filters;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Function;

public class FunctionController(
    IUserControlSession session,
    IRoleService roleService,
    IFunctionService functionService) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.FunctionManagement)]
    public ActionResult Index()
    {
        return View();
    }

    [HrmPermission(Policy.Read, FunctionKey.FunctionManagement)]
    public async Task<IActionResult> GetRolePermission([FromBody] GetRolePermissionViewModel request)
    {
        var systemRoles = await roleService.GetBySiteIdAsync(session.SiteId);
        var functionTrees = new TreeFunctionViewModel
        {
            RoleId = request.RoleId ?? systemRoles[0].RoleId,
            IsHost = session.HasPermissionOnFunction(FunctionKey.FunctionManagement, Policy.Host),
            Functions = await functionService.GetBySiteIdAsync(session.SiteId),
            HierarchyFunctions = await functionService.GetHierarchyBySiteIdAsync(session.SiteId),
            Permissions = await functionService.GetPermissionBySiteAndRoleIdAsync(session.SiteId, request.RoleId ?? systemRoles[0].RoleId)
        };

        return new JsonResult(functionTrees);
    }

    [HrmPermission(Policy.Host, FunctionKey.FunctionManagement)]
    public async Task<IActionResult> GetSystemRoles()
    {
        var systemRoles = await roleService.GetBySiteIdAsync(session.SiteId);
        return new JsonResult(systemRoles);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.FunctionManagement)]
    public async Task<JsonResult> AssignPermission([FromBody] TreeFunctionViewModel request)
    {
        var commandResult = await roleService.AssignPermissionAsync(session.SiteId, request.RoleId, request.Permissions);
        return new JsonResult(commandResult);
    }
}