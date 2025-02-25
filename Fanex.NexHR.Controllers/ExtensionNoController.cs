namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Criteria.ExtensionNumber;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.ExtensionNumber;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class ExtensionNoController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IExtensionNumberService extensionNumberService;
        private readonly ICommonDataService commonDataService;
        private readonly IBaseService baseService;

        public ExtensionNoController(
            IUserControlSession userControlSession,
            IExtensionNumberService extensionNumberService,
            ICommonDataService commonDataService,
            IBaseService baseService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.extensionNumberService = extensionNumberService;
            this.commonDataService = commonDataService;
            this.baseService = baseService;
        }

        [HRMPermission(Policy.Read, FunctionKey.ExtensionNumber)]
        public ActionResult Index()
        {
            var hasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.ExtensionNumber, Policy.Host);

            return View(hasPermission);
        }

        [HRMPermission(Policy.Read, FunctionKey.ExtensionNumber)]
        public ActionResult GetExtensionNo()
        {
            IEnumerable<ExtensionNoModel> extensionNos = extensionNumberService.GetAllExtensionNo();

            var model = extensionNos.GroupBy(item => item.TeamName)
                .Select(item => new ExtensionNumberViewModel { Name = item.Key, Extensions = item.ToList() }).ToList();
            return new CustomJsonResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult Create(ExtensionNoModel model)
        {
            var commandResult = new CommandResult();
            if (model != null)
            {
                CheckExternalEmployee(model);
                if (ModelState.IsValid)
                {
                    var criteria = new InsertExtensionNumberCriteria
                    {
                        FullName = model.FullName,
                        NickName = model.NickName,
                        ExtensionNo = model.ExtensionNo,
                        IsInternal = model.IsInternal,
                        EmpId = model.EmpId,
                        TeamId = model.TeamId,
                    };

                    commandResult = baseService.ExecuteNonQuery(criteria);
                }
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult GetExtentionById(int id, bool isInternal)
        {
            ExtensionNoModel model = extensionNumberService.GetEmployeeExtensionNoById(id, isInternal);
            return new CustomJsonResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult Edit(int id, bool isInternal)
        {
            ViewBag.Id = id;
            ViewBag.IsInternal = isInternal;

            return View();
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult Edit(ExtensionNoModel model)
        {
            var commandResult = new CommandResult();
            if (model != null)
            {
                CheckExternalEmployee(model);
                if (ModelState.IsValid)
                {
                    var criteria = new UpdateExtensionNumberMemberCriteria
                    {
                        FullName = model.FullName,
                        NickName = model.NickName,
                        ExtensionNo = model.ExtensionNo,
                        IsInternal = model.IsInternal,
                        ExtId = model.EmployeeExtensionId,
                        EmpId = model.EmpId,
                        TeamId = model.TeamId
                    };
                    commandResult = baseService.ExecuteNonQuery(criteria);
                }
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public CustomJsonResult Delete(int id)
        {
            CommandResult commandResult = extensionNumberService.DeleteExtensionNumber(id);
            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #region External Group

        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult ExternalGroup()
        {
            return View();
        }

        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public ActionResult GetAllExternalGroup()
        {
            IEnumerable<ExtensionNoModel> groups = extensionNumberService.GetAllExtensionGroupInfo(isInternal: false);

            return new CustomJsonResult { Data = groups, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public CustomJsonResult GetGroupExtension(bool isInternal, int? teamId)
        {
            IList<SelectListItem> groups;
            if (isInternal)
            {
                IList<Team> teams = commonDataService.GetAllTeam();
                groups = teams.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            }
            else
            {
                IEnumerable<ExtensionNoModel> extensionGroups = extensionNumberService.GetAllExtensionGroupInfo(teamId, isInternal);
                groups = extensionGroups.Select(x => new SelectListItem
                {
                    Text = x.TeamName,
                    Value = x.TeamId.ToString()
                }).ToList();
            }

            return new CustomJsonResult { Data = groups, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public CustomJsonResult CreateExternalGroup(string teamName)
        {
            CommandResult commandResult = extensionNumberService.CreateExternalGroup(teamName);
            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public CustomJsonResult DeleteExternalGroup(int id)
        {
            CommandResult commandResult = extensionNumberService.DeleteExternalGroup(id);
            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ExtensionNumber)]
        public CustomJsonResult EditExternalGroup(int teamId, string teamName)
        {
            CommandResult commandResult = extensionNumberService.EditExternalGroup(teamId, teamName);
            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion External Group

        private void CheckExternalEmployee(ExtensionNoModel model)
        {
            if (!model.IsInternal)
            {
                var propertyNames = ModelState.Select(x => x.Key).Where(x => x.Contains("EmpId")).ToList();
                foreach (var propertyName in propertyNames)
                {
                    ModelState[propertyName].Errors.Clear();
                }
            }
        }
    }
}