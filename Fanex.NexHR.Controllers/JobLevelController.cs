namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.JobLevel;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class JobLevelController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IJobLevelService jobLevelService;

        public JobLevelController(
            IUserControlSession userControlSession,
            IJobLevelService jobLevelService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.jobLevelService = jobLevelService;
        }

        [HRMPermission(Policy.Read, FunctionKey.JobLevelManagement)]
        public ActionResult Index()
        {
            var hasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.JobLevelManagement, Policy.Host);
            return View(hasPermission);
        }

        [ValidateInput(false)]
        [HRMPermission(Policy.Read, FunctionKey.JobLevelManagement)]
        public ActionResult GetJobLevels()
        {
            IList<JobLevel> jobLevel = jobLevelService.GetJobLevel();
            return new CustomJsonResult { Data = jobLevel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.JobLevelManagement)]
        public JsonResult Insert(string name)
        {
            var criteria = new InsertJobLevelCriteria
            {
                Name = name,
                ModifiedBy = EmployeeSession.EmpId
            };

            CommandResult result = jobLevelService.Insert(criteria);

            return new CustomJsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [ValidateInput(false)]
        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.JobLevelManagement)]
        public JsonResult Update(int id, string name)
        {
            var criteria = new UpdateJobLevelCriteria
            {
                JobLevelId = id,
                Name = name,
                ModifiedBy = EmployeeSession.EmpId
            };
            CommandResult result = jobLevelService.Update(criteria);

            return new CustomJsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.JobLevelManagement)]
        public JsonResult Delete(int id)
        {
            CommandResult result = jobLevelService.Delete(id, EmployeeSession.EmpId);

            return new CustomJsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}