namespace HRM.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Models;
    using Core.Services;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Filters;
    using HRM.Criteria.AnnualLeave;
    using HRM.Models;
    using HRM.Models.AnnualLeaveBalance;
    using HRM.Resources;
    using HRM.Services;
    using HRM.ViewModels;
    using HRM.ViewModels.AnnualLeave;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Microsoft.AspNetCore.Http;

    public class AnnualLeaveBalanceController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IEmployeeService employeeService;
        private readonly ITeamService teamService;
        private readonly IReportService reportService;
        private readonly IBaseService baseService;
        private readonly IAnnualLeaveBalanceService annualLeaveBalanceService;

        public AnnualLeaveBalanceController(
            IUserControlSession userControlSession,
            IEmployeeService employeeService,
            ITeamService teamService,
            IReportService reportService,
            IBaseService baseService,
            IAnnualLeaveBalanceService annualLeaveBalanceService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.employeeService = employeeService;
            this.teamService = teamService;
            this.reportService = reportService;
            this.baseService = baseService;
            this.annualLeaveBalanceService = annualLeaveBalanceService;
        }

        [HrmPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public IActionResult Index(
            string teamIds,
            string employee,
            int? year = null,
            int? employeeId = null,
            int? category = null)
        {
            ViewBag.HasPermissionToEdit = userControlSession.HasPermissionOnFunction(FunctionKey.AnnualLeaveBalance, Policy.Host);
            var model = new LeaveSummaryViewModel
            {
                Year = year ?? DateTimeOffset.Now.Year,
                TeamIds = teamIds,
                Employee = employee,
                EmployeeId = employeeId ?? -1,
                Category = category,
                IsShowCloseBalance = ConfigSettings.IsShowCloseBalance
            };

            return Index(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Index(LeaveSummaryViewModel model)
        {
            model.Teams = teamService.GetBySupervisor(EmployeeSession.EmpId);
            model.TeamIds = !string.IsNullOrWhiteSpace(model.TeamIds) ? model.TeamIds : string.Join(",", model.Teams.Select(x => x.Id));
            model.IsShowCloseBalance = ConfigSettings.IsShowCloseBalance;
            model.LeaveSumaries = GetAnnualLeaveBalance(model);

            return View(model);
        }

        [HttpPost]
        public JsonResult Adjust([FromBody] string annualLeaveJson)
        {
            var model = JsonConvert.DeserializeObject<AnnualLeaveBalanceMultiViewModel>(annualLeaveJson);
            var annualLeave = new AdjustAnnualLeaveBalance
            {
                EmployeeIds = model.EmpIds,
                Description = model.Description,
                CreditDays = model.CreditDays,
                DeductDays = model.DeductDays,
                AffectedFrom = model.AffectedFrom
            };
            CommandResult commandResult = annualLeaveBalanceService.Adjust(EmployeeSession.EmpId, annualLeave);

            return Json(commandResult);
        }

        [HttpPost]
        public JsonResult Reset(int minbalance, string descriptionReset)
        {
            var date = new DateTimeOffset(DateTimeOffset.Now.Year, 1, 1, 0, 0, 0, TimeSpan.Zero).AddDays(-1).DateTime;
            var teams = teamService.GetBySupervisor(EmployeeSession.EmpId);
            var criteria = new GetAnnualLeaveBalanceCriteria
            {
                Year = date.Year,
                EmpId = null,
                TeamIds = string.Join(",", teams.Select(x => x.Id)),
                IsLocal = null,
                Role = string.Empty,
                SupervisorId = null
            };
            IList<LeaveSummary> entityList = reportService.GetAnnualLeaveBalanceReport(criteria);
            var resetAnnualLeaveCriteria = new ResetAnnualLeaveCriteria
            {
                ActorId = EmployeeSession.EmpId,
                Description = descriptionReset,
                AnnualLeaves = entityList.Where(m => m.LeaveBalance > minbalance).ToList(),
                FromDate = date,
                ResetDays = minbalance
            };
            CommandResult result = baseService.ExecuteNonQuery(resetAnnualLeaveCriteria);

            return Json(result);
        }

        public IActionResult MultipleInsert()
        {
            var model = new AnnualLeaveBalanceMultiViewModel
            {
                AffectedFrom = DateTimeOffset.Now.DateTime
            };
            return PartialView("MultipleInsert", model);
        }

        public IActionResult MultipleReset()
        {
            var model = new AnnualLeaveBalanceMultiViewModel
            {
                AffectedFrom = DateTimeOffset.Now.DateTime
            };
            return PartialView(model);
        }

        [HrmPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Employee(int id)
        {
            var year = DateTimeOffset.Now.Year;
            var model = new AnnualLeaveBalanceViewModel
            {
                EmployeeId = id,
                Year = year,
                ExcludedRequestTrans = true,
                AnnualLeaveBalances = GetAnnualLeaveTransactions(id, false, year),
                Employee = employeeService.GetEmployeeById(id)
            };

            return View(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Employee(AnnualLeaveBalanceViewModel model)
        {
            model.Employee = employeeService.GetEmployeeById(model.EmployeeId);
            model.AnnualLeaveBalances = GetAnnualLeaveTransactions(model.EmployeeId, !model.ExcludedRequestTrans, model.Year);

            return View(model);
        }

        [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Edit(int id)
        {
            AnnualLeaveTransactionViewModel model = employeeService.GetAnnualLeaveTransaction(id);
            ViewBag.Employee = employeeService.GetEmployeeById(model.EmpId);
            return View(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Edit(IFormCollection formCollection, AnnualLeaveTransactionViewModel model)
        {
            ViewBag.Employee = employeeService.GetEmployeeById(model.EmpId);
            if (formCollection["command"].Equals(Labels.Delete))
            {
                return Delete(model);
            }

            return Edit(model);
        }

        [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Create(int id)
        {
            ViewBag.Employee = employeeService.GetEmployeeById(id);
            var model = new AnnualLeaveTransactionViewModel
            {
                EmpId = id,
                AffectedFrom = DateTimeOffset.Now.DateTime,
            };
            return View(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public ActionResult Create(AnnualLeaveTransactionViewModel model)
        {
            ViewBag.Employee = employeeService.GetEmployeeById(model.EmpId);
            if (ModelState.IsValid)
            {
                var criteria = new InsertAnnualLeaveTransactionCriteria
                {
                    EmpId = model.EmpId,
                    RequestId = null,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.DeductDays,
                    CreditDays = model.CreditDays,
                    CreatedDate = DateTimeOffset.Now.DateTime,
                    CreatedBy = EmployeeSession.EmpId
                };
                CommandResult result = baseService.ExecuteNonQuery(criteria);

                if (result.IsSuccessful)
                {
                    return RedirectToAction("Employee", "AnnualLeaveBalance", new { id = model.EmpId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        private IList<LeaveSummary> GetAnnualLeaveBalance(LeaveSummaryViewModel model)
        {
            var criteria = new GetAnnualLeaveBalanceCriteria
            {
                Year = model.Year,
                EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                TeamIds = model.TeamIds,
                IsLocal = (byte?)model.Category,
                Role = string.Empty,
                SupervisorId = EmployeeSession.EmpId
            };
            IList<LeaveSummary> leaveSumaries = reportService.GetAnnualLeaveBalanceReport(criteria);
            return leaveSumaries;
        }

        private IList<AnnualLeaveTransactionViewModel> GetAnnualLeaveTransactions(int empId, bool excludedRequest, int year)
        {
            var criteria = new GetAnnualLeaveTransactionCriteria
            {
                EmpId = empId,
                ExcludedRequestTrans = excludedRequest,
                Year = year
            };

            return employeeService.GetAnnualLeaveTransactions(criteria);
        }

        private ActionResult Edit(AnnualLeaveTransactionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new UpdateAnnualLeaveTransactionCriteria
                {
                    LeaveTransId = model.LeaveTransId,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.DeductDays,
                    CreditDays = model.CreditDays,
                    ModifiedDate = DateTimeOffset.Now.DateTime,
                    ModifiedBy = EmployeeSession.EmpId
                };
                CommandResult result = baseService.ExecuteNonQuery(criteria);

                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return View(model);
        }

        private ActionResult Delete(AnnualLeaveTransactionViewModel model)
        {
            var criteria = new DeleteAnnualLeaveTransactionCriteria
            {
                LeaveTransId = model.LeaveTransId,
                ModifiedDate = DateTimeOffset.Now.DateTime,
                ModifiedBy = EmployeeSession.EmpId
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            if (result.IsSuccessful)
            {
                return RedirectToAction("Employee", "AnnualLeaveBalance", new { id = model.EmpId });
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }
        }
    }
}