namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Core.Models;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Criteria.Unit;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Unit;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class UnitController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IUnitService unitService;

        public UnitController(
            IUserControlSession userControlSession,
            IUnitService unitService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.unitService = unitService;
        }

        [HRMPermission(Policy.Read, FunctionKey.UnitManagement)]
        public ActionResult Index()
        {
            var hasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.UnitManagement, Policy.Host);
            return View(hasPermission);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.UnitManagement)]
        public ActionResult GetUnits(UnitViewModel model)
        {
            var units = unitService.GetUnit(model.IsActive);
            return new CustomJsonResult { Data = units, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public JsonResult Create(AddUnitViewModel model)
        {
            var commandResult = new CommandResult();
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertUnitCriteria
                {
                    Code = model.Code,
                    Name = model.Name,
                    IsActive = model.IsActive,
                    CreatedBy = EmployeeSession.EmpId
                };
                commandResult = unitService.Create(criteria);
            }
            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public ActionResult GetById(int id)
        {
            Unit unit = unitService.GetById(id);
            return new CustomJsonResult { Data = unit, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public ActionResult Edit(int id)
        {
            return View(id);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public JsonResult Edit(EditUnitViewModel model)
        {
            var commandResult = new CommandResult();
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateUnitCriteria
                {
                    UnitId = model.UnitId,
                    Code = model.Code,
                    Name = model.Name,
                    IsActive = model.IsActive,
                    ModifiedBy = EmployeeSession.EmpId
                };
                commandResult = unitService.Edit(criteria);
            }
            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.UnitManagement)]
        public JsonResult Delete(DeleteUnitCriteria criteria)
        {
            criteria.ModifiedBy = EmployeeSession.EmpId;
            var commandResult = unitService.Delete(criteria);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}