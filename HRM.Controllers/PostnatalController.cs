using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
    using Core.Models;
    using Criteria.Postnatal;
    using HRM.Core.Filters;
    using HRM.Models;
    using HRM.Services;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using ViewModels.Postnatal;

    public class PostnatalController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IPostnatalService postnatalService;

        public PostnatalController(
            IUserControlSession userControlSession,
            IPostnatalService postnatalService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.postnatalService = postnatalService;
        }

        [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult Index()
        {
            ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
            return View();
        }

        [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult GetPostnatalGrid(int year)
        {
            IList<EmployeePostnatal> postnatals = postnatalService.GetAll(year);
            ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
            return PartialView("_PostnatalGrid", postnatals);
        }

        [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create()
        {
            var currentDateTime = DateTimeOffset.Now.DateTime;
            var model = new PostnatalViewModel
            {
                FromDate = currentDateTime,
                ToDate = currentDateTime
            };
            return PartialView("_Create", model);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create(PostnatalViewModel model)
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

                result = postnatalService.Insert(criteria);
            }
            else
            {
                result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any()).Errors.FirstOrDefault().ErrorMessage;
            }

            return Json(result);
        }

        [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(int id)
        {
            EmployeePostnatal postnatal = postnatalService.GetById(id);
            var model = new PostnatalViewModel
            {
                EmployeeId = postnatal.EmpId,
                Employee = string.Format("{0} ({1})", postnatal.Nickname, postnatal.Fullname),
                FromDate = postnatal.FromDate,
                ToDate = postnatal.ToDate,
                Remark = postnatal.Remark,
                WorkingHours = postnatal.WorkingHours
            };
            return PartialView("_Edit", model);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Update(int id, PostnatalViewModel model)
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

                result = postnatalService.Update(criteria);
            }
            else
            {
                result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any()).Errors.FirstOrDefault().ErrorMessage;
            }

            return Json(result);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Delete(int id)
        {
            var result = postnatalService.Delete(id);

            return Json(result);
        }
    }
}