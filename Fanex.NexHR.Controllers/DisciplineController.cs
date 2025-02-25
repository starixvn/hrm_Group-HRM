namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria.Discipline;
    using Fanex.NexHR.Models.Discipline;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Discipline;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class DisciplineController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IDisciplineService disciplineService;
        private readonly IBaseService baseService;

        public DisciplineController(
            IUserControlSession userControlSession,
            IDisciplineService disciplineService,
            IBaseService baseService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.disciplineService = disciplineService;
            this.baseService = baseService;
        }

        [HRMPermission(Policy.Read, FunctionKey.Discipline)]
        public ActionResult DisciplineSummary(DisciplineSummaryViewModel model)
        {
            if (model != null)
            {
                model.FromDate = GetFromDate();
                model.ToDate = GetToDate();
                model.DisciplineTypes = GetDisciplineType(true);
                model.DateRange = GetDateRangeByQuarter();
            }

            ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);
            ViewBag.IsHostOtherDiscipline = userControlSession.HasPermissionOnFunction(FunctionKey.OtherDiscipline, Policy.Host);
            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.Discipline)]
        public ActionResult GetDisciplineSummaryGrid(DisciplineSummaryViewModel model)
        {
            var criteria = new GetDisciplineSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TeamId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                ReasonId = model.ReasonId,
                ApproverId = EmployeeSession.EmpId,
                IsShowAll = model.IsShowAll
            };

            IList<DisciplineSummaryModel> disciplines = disciplineService.GetDisciplineSummary(criteria);

            ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);

            return PartialView("_DisciplineSummaryGrid", disciplines);
        }

        [HRMPermission(Policy.Read, FunctionKey.Discipline)]
        public ActionResult DisciplineDetail(DisciplineSummaryViewModel model)
        {
            if (model != null)
            {
                if (model.FromDate == default(DateTime))
                {
                    model.FromDate = GetFromDate();
                    model.ToDate = GetToDate();
                }

                if (model.ReasonId < 0)
                {
                    model.ReasonId = null;
                }

                model.DisciplineTypes = GetDisciplineType(false);
                model.DateRange = GetDateRangeByQuarter();
            }

            return View(model);
        }

        [HRMPermission(Policy.Read, FunctionKey.Discipline)]
        public ActionResult GetDisciplineDetailGrid(DisciplineSummaryViewModel model)
        {
            var criteria = new GetDisciplineDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                TeamId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                ReasonId = model.ReasonId,
                HasIgnored = model.HasIgnored,
                ApproverId = EmployeeSession.EmpId
            };

            IList<DisciplineDetailModel> disciplines = disciplineService.GetDisciplineDetail(criteria);
            ViewBag.HasPermissionToEdit = userControlSession.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);
            ViewBag.HasPermissionToUpdateOutOfControlReason = userControlSession.HasPermissionOnFunction(FunctionKey.UpdateOutOfControlReason, Policy.Host);

            return PartialView("_DisciplineDetailGrid", disciplines);
        }

        [HRMPermission(Policy.Read, FunctionKey.Discipline)]
        public ActionResult ViewOtherDiscipline(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetOtherDisciplineOfEmployeeCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };

            IList<OtherDisciplineModel> otherDisciplines = disciplineService.GetOtherDiscipline(criteria);
            IEnumerable<EmployeeInfoModel> members = disciplineService.GetMemberToAddOtherDiscipline(EmployeeSession.EmpId);
            ViewBag.IsHost = members.Any(x => x.EmployeeId == employeeId)
                && userControlSession.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);

            return PartialView("_ViewOtherDiscipline", otherDisciplines);
        }

        [HRMPermission(Policy.Host, FunctionKey.OtherDiscipline)]
        public ActionResult PartialAddOtherDiscipline(string employee, int? employeeId = null)
        {
            var criteria = new GetOtherDisciplineTypeCriteria();
            IList<DisciplineTypeModel> otherDisciplineTypes = disciplineService.GetOtherDisciplineType(criteria);

            var model = new AddOtherDisciplineViewModel
            {
                EmployeeId = employeeId ?? -1,
                Employee = employee,
                WorkDate = DateTimeOffset.Now.DateTime,
                OtherDisciplineTypes = otherDisciplineTypes
            };

            return PartialView("_AddOtherDiscipline", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [HRMPermission(Policy.Host, FunctionKey.OtherDiscipline)]
        public JsonResult AddOtherDiscipline(AddOtherDisciplineViewModel model)
        {
            var criteria = new InsertOtherDisciplineCriteria
            {
                EmployeeId = model.EmployeeId,
                WorkDate = model.WorkDate,
                DisciplineTypeId = model.DisciplineTypeId,
                Score = model.MinusScore,
                Comment = model.Comment,
                ActorId = EmployeeSession.EmpId
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return Json(commandResult);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.OtherDiscipline)]
        public string DeleteOtherDiscipline(DeleteOtherDisciplineCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ActorId = EmployeeSession.EmpId;
                CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

                if (!commandResult.IsSuccessful)
                {
                    return commandResult.Message;
                }
            }

            return string.Empty;
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.Discipline)]
        public CustomJsonResult EditDisciplineIgnore(IEnumerable<IgnoredDisciplineModel> ignoreDetails,
            IEnumerable<IgnoredDisciplineDetailModel> disciplineIgnores)
        {
            var criteria = new UpdateIgnoredDisciplineCriteria
            {
                IgnoreDetailXml = ignoreDetails.ToXML().ToString(),
                DisciplineIgnoreXml = disciplineIgnores.ToXML().ToString(),
                HasUpdateOutOfControlReason = userControlSession.HasPermissionOnFunction(FunctionKey.UpdateOutOfControlReason, Policy.Host),
                ActorId = EmployeeSession.EmpId,
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetMemberToAddOtherDiscipline(string keyword)
        {
            IEnumerable<EmployeeInfoModel> employees = disciplineService.GetMemberToAddOtherDiscipline(EmployeeSession.EmpId, keyword);

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        private static IList<SelectListItem> GetDateRangeByQuarter()
        {
            var dateRange = new List<SelectListItem> { new SelectListItem { Text = Labels.DateRange, Value = "0" } };
            var currentDate = DateTimeOffset.Now.Date;
            const byte May = 5;
            const byte June = 6;
            const byte December = 12;
            for (int i = 0; i < June; i++)
            {
                int currentMonth = currentDate.Month;
                int currentYear = currentDate.Year;
                if (currentMonth == December)
                {
                    dateRange.Add(new SelectListItem
                    {
                        Text = string.Format("1H - {0}", currentYear + 1),
                        Value = string.Format("1{0}", currentYear + 1)
                    });
                }
                else if (currentMonth >= 1 && currentMonth <= May)
                {
                    dateRange.Add(new SelectListItem
                    {
                        Text = string.Format("1H - {0}", currentYear),
                        Value = string.Format("1{0}", currentYear)
                    });
                }
                else
                {
                    dateRange.Add(new SelectListItem
                    {
                        Text = string.Format("2H - {0}", currentYear),
                        Value = string.Format("2{0}", currentYear)
                    });
                }

                currentDate = currentDate.AddMonths(-June);
            }

            return dateRange;
        }

        private static DateTime GetFromDate()
        {
            const byte May = 5;
            const byte June = 6;
            const byte December = 12;
            var today = DateTimeOffset.Now.Date;
            var month = today.Month;
            DateTimeOffset fromDate;
            if (month == December)
            {
                fromDate = new DateTimeOffset(today.Year, month, 1, 0, 0, 0, TimeSpan.Zero);
            }
            else if (month >= 1 && month <= May)
            {
                fromDate = new DateTimeOffset(today.Year - 1, December, 1, 0, 0, 0, TimeSpan.Zero);
            }
            else
            {
                fromDate = new DateTimeOffset(today.Year, June, 1, 0, 0, 0, TimeSpan.Zero);
            }

            return fromDate.DateTime;
        }

        private static DateTime GetToDate()
        {
            const byte May = 5;
            const byte December = 12;
            var today = DateTimeOffset.Now.Date;
            var month = today.Month;
            DateTimeOffset toDate;
            if (month == December)
            {
                toDate = new DateTimeOffset(today.Year + 1, May, DateTime.DaysInMonth(today.Year + 1, May), 0, 0, 0, TimeSpan.Zero);
            }
            else if (month >= 1 && month <= May)
            {
                toDate = new DateTimeOffset(today.Year, May, DateTime.DaysInMonth(today.Year, May), 0, 0, 0, TimeSpan.Zero);
            }
            else
            {
                toDate = new DateTimeOffset(today.Year, December - 1, DateTime.DaysInMonth(today.Year, December - 1), 0, 0, 0, TimeSpan.Zero);
            }

            return toDate.DateTime;
        }

        private IList<DisciplineTypeModel> GetDisciplineType(bool includeOtherType)
        {
            var criteria = new GetDisciplineTypeCriteria
            {
                IncludeOtherType = includeOtherType
            };
            IList<DisciplineTypeModel> disciplineTypes = disciplineService.GetDisciplineType(criteria);

            return disciplineTypes;
        }
    }
}