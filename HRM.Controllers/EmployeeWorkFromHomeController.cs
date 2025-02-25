using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Core.Constants;
    using Core.Models;
    using HRM.Core;
    using HRM.Core.Filters;
    using HRM.Core.Services;
    using HRM.Criteria.EmployeeWorkFromHome;
    using HRM.Models;
    using HRM.Services;
    using HRM.ViewModels.EmployeeWorkFromHome;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using Newtonsoft.Json;
    using Microsoft.AspNetCore.Http;

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

        [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
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

        [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
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

        [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
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
        [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Create(string requestModel, IList<IFormFile> files)
        {
            var model = JsonConvert.DeserializeObject<EmployeeWorkFromHomeViewModel>(requestModel);
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

        [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
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
        [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Edit(string requestModel, IList<IFormFile> files, string filesRemove)
        {
            var model = JsonConvert.DeserializeObject<EmployeeWorkFromHomeViewModel>(requestModel);
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
                        attachFileService.DeleteFile(JsonConvert.DeserializeObject<List<string>>(filesRemove), criteria.EmployeeWorkFromHomeId, UploadFolderPath);
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
        [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
        public ActionResult Delete(int id)
        {
            var criteria = new DeleteEmployeeWorkFromHomeCriteria { EmployeeWorkFromHomeId = id };
            var result = baseService.ExecuteNonQuery(criteria);

            return Json(result);
        }
    }
}