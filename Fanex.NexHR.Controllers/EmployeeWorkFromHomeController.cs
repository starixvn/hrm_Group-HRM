namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Core.Constants;
    using Core.Models;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.EmployeeWorkFromHome;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class EmployeeWorkFromHomeController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IEmployeeWorkFromHomeService employeeWorkFromHomeService;
        private readonly IBaseService baseService;
        private readonly IAttachFileService attachFileService;
        private const string UploadFolderPath = "/file/uploads/wfh/";

        public EmployeeWorkFromHomeController(
            IUserControlSession userControlSession,
            IBaseService baseService,
            IAttachFileService attachFileService,
            IEmployeeWorkFromHomeService employeeWorkFromHomeService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.baseService = baseService;
            this.employeeWorkFromHomeService = employeeWorkFromHomeService;
            this.attachFileService = attachFileService;
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult Index()
        {
            var currentDateTime = DateTimeOffset.Now.DateTime;
            var model = new EmployeeWorkFromHomeFilterViewModel
            {
                FromDateFilter = currentDateTime,
                ToDateFilter = currentDateTime
            };

            ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
            return View(model);
        }

        [HRMPermission(Policy.Read, FunctionKey.EmployeeManagement)]
        public ActionResult GetEmployeeWorkFromHomeGrid(GetEmployeeWorkFromHomeCriteria criteria)
        {
            var employees = baseService.GetData<EmployeeWorkFromHome>(criteria);
            foreach (var employee in employees)
            {
                employee.AttachFiles = attachFileService.GetInfoAttachFile(employee.EmployeeWorkFromHomeId, UploadFolderPath);
            }

            ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
            return PartialView("_EmployeeWorkFromHomeGrid", employees);
        }

        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create()
        {
            var currentDateTime = DateTimeOffset.Now.DateTime;
            var model = new EmployeeWorkFromHomeViewModel
            {
                FromDate = currentDateTime,
                ToDate = currentDateTime
            };
            return PartialView("_Create", model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create(string requestModel, IList<HttpPostedFileBase> files)
        {
            var model = JsonConverter.FromJson<EmployeeWorkFromHomeViewModel>(requestModel);
            var result = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertEmployeeWorkFromHomeCriteria
                {
                    EmpId = model.EmployeeId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    CreatedBy = EmployeeSession.EmpId,
                    Remark = model.Remark
                };

                result = baseService.ExecuteNonQuery(criteria);
                if (result.IsSuccessful)
                {
                    attachFileService.UploadFile(files, criteria.RequestId, UploadFolderPath);
                }
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
            var employeeWorkFromHome = employeeWorkFromHomeService.GetById(id);
            var model = new EmployeeWorkFromHomeViewModel
            {
                EmployeeWorkFromHomeId = employeeWorkFromHome.EmployeeWorkFromHomeId,
                EmployeeId = employeeWorkFromHome.EmpId,
                Employee = string.Format("{0} ({1})", employeeWorkFromHome.Nickname, employeeWorkFromHome.FullName),
                FromDate = employeeWorkFromHome.FromDate,
                ToDate = employeeWorkFromHome.ToDate,
                Remark = employeeWorkFromHome.Remark,
                Files = attachFileService.GetAttachFile(id, UploadFolderPath)
            };
            return PartialView("_Edit", model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(string requestModel, IList<HttpPostedFileBase> files, string filesRemove)
        {
            var model = JsonConverter.FromJson<EmployeeWorkFromHomeViewModel>(requestModel);
            var result = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateEmployeeWorkFromHomeCriteria
                {
                    EmployeeWorkFromHomeId = model.EmployeeWorkFromHomeId,
                    EmpId = model.EmployeeId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ModifiedBy = EmployeeSession.EmpId,
                    Remark = model.Remark
                };

                result = baseService.ExecuteNonQuery(criteria);
                if (result.IsSuccessful)
                {
                    if (!string.IsNullOrWhiteSpace(filesRemove))
                    {
                        attachFileService.DeleteFile(JsonConverter.FromJsonArray<string>(filesRemove), criteria.EmployeeWorkFromHomeId, UploadFolderPath);
                    }
                    attachFileService.UploadFile(files, criteria.EmployeeWorkFromHomeId, UploadFolderPath);
                }
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
            var criteria = new DeleteEmployeeWorkFromHomeCriteria { EmployeeWorkFromHomeId = id };
            var result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }
    }
}