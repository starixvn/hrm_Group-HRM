namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Department;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class DepartmentController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IDepartmentService departmentService;
        private readonly IBaseService baseService;
        private readonly IUnitService unitService;

        public DepartmentController(
            IUserControlSession userControlSession,
            IDepartmentService departmentService,
            IUnitService unitService,
            IBaseService baseService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.departmentService = departmentService;
            this.baseService = baseService;
            this.unitService = unitService;
        }

        [HRMPermission(Policy.Read, FunctionKey.DepartmentManagement)]
        public ActionResult Index()
        {
            var hasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.DepartmentManagement, Policy.Host);
            return View(hasPermission);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.DepartmentManagement)]
        public ActionResult GetDepartments(bool? isActive = null)
        {
            var departments = departmentService.GetDepartment(isActive);
            return new CustomJsonResult { Data = departments, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetUnits()
        {
            var units = unitService.GetUnit();
            return new CustomJsonResult { Data = units, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HRMPermission(Policy.Host, FunctionKey.DepartmentManagement)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.DepartmentManagement)]
        public JsonResult Create(CreateDepartmentViewModel model)
        {
            var commandResult = new CommandResult();
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertDepartmentCriteria
                {
                    Code = model.Code,
                    Name = model.Name,
                    IsActive = model.IsActive,
                    UnitId = model.UnitId,
                    ModifiedBy = EmployeeSession.EmpId
                };
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HRMPermission(Policy.Host, FunctionKey.DepartmentManagement)]
        public ActionResult GetById(int Id)
        {
            Department department = departmentService.GetById(Id);
            IList<Unit> units = unitService.GetUnit();
            var model = new EditDepartmentViewModel
            {
                DepartmentId = Id,
                Code = department.Code,
                Name = department.Name,
                IsActive = department.IsActive,
                UnitId = department.Unit != null ? department.Unit.Id : 0,
                Units = units
            };

            return new CustomJsonResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HRMPermission(Policy.Host, FunctionKey.DepartmentManagement)]
        public ActionResult Edit(int id)
        {
            return View(id);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.DepartmentManagement)]
        public JsonResult Edit(EditDepartmentViewModel model)
        {
            var commandResult = new CommandResult();
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateDepartmentCriteria
                {
                    DepartmentId = model.DepartmentId,
                    Code = model.Code,
                    Name = model.Name,
                    IsActive = model.IsActive,
                    UnitId = model.UnitId,
                    ModifiedBy = EmployeeSession.EmpId
                };
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.DepartmentManagement)]
        public CustomJsonResult Delete(int id)
        {
            CommandResult commandResult = departmentService.Delete(EmployeeSession.EmpId, id);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}