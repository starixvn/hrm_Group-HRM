namespace HRM.Controllers;

using GO.UserControl.Client.Core.Services;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using Core.Constants;
using Core.Filters;
using ViewModels.Function;
using Microsoft.AspNetCore.Mvc;


public class FunctionController(IUserControlSession userControlSession,
    IRoleService roleService,
    IFunctionService functionService) : BaseController(userControlSession)
{
    private readonly IUserControlSession userControlSession = userControlSession;

    [HrmPermission(Policy.Read, FunctionKey.FunctionManagement)]
    public ActionResult Index()
    {
        return View();
    }

    [HrmPermission(Policy.Read, FunctionKey.FunctionManagement)]
    public async Task<IActionResult> GetRolePermission([FromBody] GetRolePermissionViewModel request)
    {
        var systemRoles = await roleService.GetBySiteIdAsync(userControlSession.SiteId);
        var functionTrees = new TreeFunctionViewModel
        {
            RoleId = request.RoleId ?? systemRoles[0].RoleId,
            IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.FunctionManagement, Policy.Host),
            Functions = await functionService.GetBySiteIdAsync(userControlSession.SiteId),
            HierarchyFunctions = await functionService.GetHierarchyBySiteIdAsync(userControlSession.SiteId),
            Permissions = await functionService.GetPermissionBySiteAndRoleIdAsync(userControlSession.SiteId, request.RoleId ?? systemRoles[0].RoleId)
        };

        return new JsonResult(functionTrees);
    }

    [HrmPermission(Policy.Host, FunctionKey.FunctionManagement)]
    public async Task<IActionResult> GetSystemRoles()
    {
        var systemRoles = await roleService.GetBySiteIdAsync(userControlSession.SiteId);
        return new JsonResult(systemRoles);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.FunctionManagement)]
    public async Task<JsonResult> AssignPermission([FromBody] TreeFunctionViewModel request)
    {
        var commandResult = await roleService.AssignPermissionAsync(userControlSession.SiteId, request.RoleId, request.Permissions);
        return new JsonResult(commandResult);
    }
}