namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using HRM.Controllers;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Filters;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Core.XMLExtensions;
    using HRM.Criteria.Discipline;
    using HRM.Models.Discipline;
    using HRM.Resources;
    using HRM.Services;
    using HRM.ViewModels.Discipline;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

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

        [HrmPermission(Policy.Read, FunctionKey.Discipline)]
        public async Task<IActionResult> DisciplineSummary(DisciplineSummaryViewModel model)
        {
            if (model != null)
            {
                model.FromDate = GetFromDate();
                model.ToDate = GetToDate();
                model.DisciplineTypes = await GetDisciplineType(true);
                model.DateRange = GetDateRangeByQuarter();
            }

            ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);
            ViewBag.IsHostOtherDiscipline = userControlSession.HasPermissionOnFunction(FunctionKey.OtherDiscipline, Policy.Host);
            return View(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Read, FunctionKey.Discipline)]
        public async Task<IActionResult> GetDisciplineSummaryGrid(DisciplineSummaryViewModel model)
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

            var disciplines = await disciplineService.GetDisciplineSummaryAsync(criteria);

            ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);

            return PartialView("_DisciplineSummaryGrid", disciplines);
        }

        [HrmPermission(Policy.Read, FunctionKey.Discipline)]
        public async Task<IActionResult> DisciplineDetail(DisciplineSummaryViewModel model)
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

                model.DisciplineTypes = await GetDisciplineType(false);
                model.DateRange = GetDateRangeByQuarter();
            }

            return View(model);
        }

        [HrmPermission(Policy.Read, FunctionKey.Discipline)]
        public async Task<IActionResult> GetDisciplineDetailGrid(DisciplineSummaryViewModel model)
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

            var disciplines = await disciplineService.GetDisciplineDetailAsync(criteria);
            ViewBag.HasPermissionToEdit = userControlSession.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);
            ViewBag.HasPermissionToUpdateOutOfControlReason = userControlSession.HasPermissionOnFunction(FunctionKey.UpdateOutOfControlReason, Policy.Host);

            return PartialView("_DisciplineDetailGrid", disciplines);
        }

        [HrmPermission(Policy.Read, FunctionKey.Discipline)]
        public async Task<IActionResult> ViewOtherDiscipline(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetOtherDisciplineOfEmployeeCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };

            var otherDisciplines = await disciplineService.GetOtherDisciplineAsync(criteria);
            var members = await disciplineService.GetMemberToAddOtherDisciplineAsync(EmployeeSession.EmpId);
            ViewBag.IsHost = members.Any(x => x.EmployeeId == employeeId)
                && userControlSession.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);

            return PartialView("_ViewOtherDiscipline", otherDisciplines);
        }

        [HrmPermission(Policy.Host, FunctionKey.OtherDiscipline)]
        public async Task<IActionResult> PartialAddOtherDiscipline(string employee, int? employeeId = null)
        {
            var criteria = new GetOtherDisciplineTypeCriteria();
            var otherDisciplineTypes = await disciplineService.GetOtherDisciplineTypeAsync(criteria);

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
        [HrmPermission(Policy.Host, FunctionKey.OtherDiscipline)]
        public async Task<IActionResult> AddOtherDiscipline(AddOtherDisciplineViewModel model)
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
            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(commandResult);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.OtherDiscipline)]
        public async Task<string> DeleteOtherDiscipline(DeleteOtherDisciplineCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ActorId = EmployeeSession.EmpId;
                var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

                if (!commandResult.IsSuccessful)
                {
                    return commandResult.Message;
                }
            }

            return string.Empty;
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.Discipline)]
        public async Task<IActionResult> EditDisciplineIgnore(IEnumerable<IgnoredDisciplineModel> ignoreDetails,
            IEnumerable<IgnoredDisciplineDetailModel> disciplineIgnores)
        {
            var criteria = new UpdateIgnoredDisciplineCriteria
            {
                IgnoreDetailXml = ignoreDetails.ToXML().ToString(),
                DisciplineIgnoreXml = disciplineIgnores.ToXML().ToString(),
                HasUpdateOutOfControlReason = userControlSession.HasPermissionOnFunction(FunctionKey.UpdateOutOfControlReason, Policy.Host),
                ActorId = EmployeeSession.EmpId,
            };
            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(commandResult);
        }

        public async Task<IActionResult> GetMemberToAddOtherDiscipline(string keyword)
        {
            var employees = await disciplineService.GetMemberToAddOtherDisciplineAsync(EmployeeSession.EmpId, keyword);

            return Json(employees);
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

        private async Task<IList<DisciplineTypeModel>> GetDisciplineType(bool includeOtherType)
        {
            var criteria = new GetDisciplineTypeCriteria
            {
                IncludeOtherType = includeOtherType
            };
            var disciplineTypes = await disciplineService.GetDisciplineTypeAsync(criteria);


            return disciplineTypes;
        }
    }
}