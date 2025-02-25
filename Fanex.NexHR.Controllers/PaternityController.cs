namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Paternity;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Paternity;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class PaternityController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IPaternityService paternityService;

        public PaternityController(
            IUserControlSession userControlSession,
            IPaternityService paternityService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.paternityService = paternityService;
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult Index()
        {
            ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
            return View();
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult ListPaternity(int year)
        {
            var paternities = paternityService.GetAll(year);
            ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
            return PartialView("_ListPaternity", paternities);
        }

        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create()
        {
            var model = new PaternityViewModel
            {
                ChildBornDate = DateTimeOffset.Now.DateTime,
                LeaveDay = 5
            };
            return PartialView("_Create", model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create(PaternityViewModel model)
        {
            var result = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertPaternityCriteria
                {
                    EmpId = model.EmployeeId,
                    ChildBornDate = model.ChildBornDate,
                    LeaveDay = model.LeaveDay,
                    CreatedBy = EmployeeSession.EmpId,
                    Remark = model.Remark
                };

                result = paternityService.Insert(criteria);
            }
            else
            {
                result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any()).Errors.FirstOrDefault().ErrorMessage;
            }

            return Json(result);
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(int id)
        {
            var paternity = paternityService.GetById(id);

            var model = new PaternityViewModel
            {
                EmployeeId = paternity.EmpId,
                Employee = string.Format("{0} ({1})", paternity.Nickname, paternity.Fullname),
                ChildBornDate = paternity.ChildBornDate,
                LeaveDay = paternity.LeaveDay,
                Remark = paternity.Remark
            };
            return PartialView("_Edit", model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Update(int id, PaternityViewModel model)
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
                    ModifiedBy = EmployeeSession.EmpId,
                    Remark = model.Remark
                };

                result = paternityService.Update(criteria);
            }
            else
            {
                result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any()).Errors.FirstOrDefault().ErrorMessage;
            }

            return Json(result);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Delete(int id)
        {
            var result = paternityService.Delete(id);

            return Json(result);
        }
    }
}