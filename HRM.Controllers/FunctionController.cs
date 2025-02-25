using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System.Collections.Generic;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Filters;
    using HRM.ViewModels.Function;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Models;
    using GO.UserControl.Client.Core.Models.Role;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;

    public class FunctionController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IRoleService roleService;
        private readonly IFunctionService ucfunctionService;

        public FunctionController(
            IUserControlSession userControlSession,
            IRoleService roleService,
            IFunctionService ucfunctionService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.roleService = roleService;
            this.ucfunctionService = ucfunctionService;
        }

        [HrmPermission(Policy.Read, FunctionKey.FunctionManagement)]
        public ActionResult Index()
        {
            return View();
        }

        [HrmPermission(Policy.Read, FunctionKey.FunctionManagement)]
        public ActionResult GetRolePermission(short? roleId = null)
        {
            IList<Role> systemRoles = roleService.GetBySiteId(userControlSession.SiteId);
            var functionTrees = new TreeFunctionViewModel
            {
                RoleId = roleId ?? systemRoles[0].RoleId,
                IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.FunctionManagement, Policy.Host),
                Functions = ucfunctionService.GetBySiteId(userControlSession.SiteId),
                HierarchyFunctions = ucfunctionService.GetHierarchyBySiteId(userControlSession.SiteId),
                Permissions = ucfunctionService.GetPermissionBySiteAndRoleId(userControlSession.SiteId, roleId ?? systemRoles[0].RoleId)
            };

            return new CustomJsonResult { Data = functionTrees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HrmPermission(Policy.Host, FunctionKey.FunctionManagement)]
        public ActionResult GetSystemRoles()
        {
            IList<Role> systemRoles = roleService.GetBySiteId(userControlSession.SiteId);
            return new CustomJsonResult { Data = systemRoles, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.FunctionManagement)]
        public CustomJsonResult AssignPermission(short roleId, IList<FunctionPermission> permissions)
        {
            CommandResult commandResult = roleService.AssignPermission(userControlSession.SiteId, roleId, permissions);
            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}