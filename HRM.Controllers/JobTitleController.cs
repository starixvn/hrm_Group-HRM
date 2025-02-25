using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System;
    using System.Collections.Generic;
    using Core.Enums;
    using Core.Models;
    using Core.Services;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Filters;
    using HRM.Criteria.Lookup;
    using HRM.Models;
    using HRM.Services;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using Resources;

    public class JobTitleController : BaseExportController
    {
        private readonly IUserControlSession userControlSession;
        private readonly ICommonDataService commonDataService;
        private readonly IJobTitleExportService jobTitleExportService;
        private readonly IBaseService baseService;

        public JobTitleController(
            IUserControlSession userControlSession,
            ICommonDataService commonDataService,
            IJobTitleExportService jobTitleExportService,
            IBaseService baseService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.commonDataService = commonDataService;
            this.jobTitleExportService = jobTitleExportService;
            this.baseService = baseService;
        }

        [HrmPermission(Policy.Read, FunctionKey.JobTitleManagement)]
        public ActionResult JobTitleManagement()
        {
            var hasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.JobTitleManagement, Policy.Host);
            return View(hasPermission);
        }

        [HrmPermission(Policy.Host, FunctionKey.JobTitleManagement)]
        public ActionResult GetJobCategory()
        {
            IList<SelectListItem> jobCategories = GetJobCategories();
            return new CustomJsonResult { Data = jobCategories, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [ValidateInput(false)]
        [HrmPermission(Policy.Read, FunctionKey.JobTitleManagement)]
        public ActionResult GetJobTitles(string jobTitle)
        {
            IList<LookupData> jobTitles = commonDataService.GetJobTitle(jobTitle);
            return new CustomJsonResult { Data = jobTitles, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HrmPermission(Policy.Host, FunctionKey.JobTitleManagement)]
        public ActionResult ExportExcel(string jobTitle)
        {
            IList<LookupData> jobTitles = commonDataService.GetJobTitle(jobTitle);
            byte[] bytes = jobTitleExportService.Export(jobTitles);

            return File(bytes, MimeType.XLSX, $"JobTitle_{DateTimeOffset.Now:yyyyMMdd}.xlsx");
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.JobTitleManagement)]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteLookupDataCriteria
            {
                Id = id
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return new CustomJsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [ValidateInput(false)]
        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.JobTitleManagement)]
        public JsonResult Insert(string name, int categoryId)
        {
            var criteria = new InsertLookupDataCriteria
            {
                LookupCode = Constants.JobTitle,
                Name = name,
                CategoryId = categoryId.ToString()
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return new CustomJsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [ValidateInput(false)]
        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.JobTitleManagement)]
        public JsonResult Update(int id, string name, int categoryId)
        {
            var criteria = new UpdateLookupDataCriteria
            {
                Id = id,
                Name = name,
                CategoryId = categoryId.ToString()
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return new CustomJsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private static IList<SelectListItem> GetJobCategories()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem { Text = Labels.Worker, Value = ((int)JobTitleCategory.Worker).ToString() },
                    new SelectListItem { Text = Labels.Staff, Value = ((int)JobTitleCategory.Staff).ToString() },
                    new SelectListItem { Text = Labels.Executive, Value = ((int)JobTitleCategory.Executive).ToString() },
                    new SelectListItem { Text = Labels.ExecutiveWithPosition, Value = ((int)JobTitleCategory.ExecutiveWithPosition).ToString() },
                    new SelectListItem { Text = Labels.Management, Value = ((int)JobTitleCategory.Management).ToString() }
                };
        }
    }
}