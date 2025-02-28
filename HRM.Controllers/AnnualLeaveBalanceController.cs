using HRM.Services.Interfaces;

namespace HRM.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Models;
    using Core.Services;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using HRM.Core.Constants;
    using HRM.Core.Filters;
    using HRM.Criteria.AnnualLeave;
    using HRM.Models;
    using HRM.Models.AnnualLeaveBalance;
    using HRM.Resources;
    using HRM.Services;
    using HRM.ViewModels;
    using HRM.ViewModels.AnnualLeave;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class AnnualLeaveBalanceController(
        AppSettings appSettings,
        IUserControlSession userControlSession,
        IEmployeeService employeeService,
        ITeamService teamService,
        IReportService reportService,
        IBaseService baseService,
        IAnnualLeaveBalanceService annualLeaveBalanceService) : BaseController(userControlSession)
    {
        [HrmPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public async Task<IActionResult> Index(
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
                IsShowCloseBalance = appSettings.IsShowCloseBalance
            };

            return await Index(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public async Task<IActionResult> Index(LeaveSummaryViewModel model)
        {
            model.Teams = await teamService.GetBySupervisorAsync(EmployeeSession.EmpId);
            model.TeamIds = !string.IsNullOrWhiteSpace(model.TeamIds) ? model.TeamIds : string.Join(",", model.Teams.Select(x => x.Id));
            model.IsShowCloseBalance = appSettings.IsShowCloseBalance;
            model.LeaveSumaries = await GetAnnualLeaveBalanceAsync(model);

            return View(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public async Task<IActionResult> Adjust([FromBody] AnnualLeaveBalanceMultiViewModel model)
        {
            var annualLeave = new AdjustAnnualLeaveBalance
            {
                EmployeeIds = model.EmpIds,
                Description = model.Description,
                CreditDays = model.CreditDays,
                DeductDays = model.DeductDays,
                AffectedFrom = model.AffectedFrom
            };
            var commandResult = await annualLeaveBalanceService.AdjustAsync(EmployeeSession.EmpId, annualLeave);

            return Json(commandResult);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public async Task<IActionResult> Reset(int minBalance, string descriptionReset)
        {
            var date = new DateTimeOffset(DateTimeOffset.Now.Year, 1, 1, 0, 0, 0, TimeSpan.Zero).AddDays(-1).DateTime;
            var teams = await teamService.GetBySupervisorAsync(EmployeeSession.EmpId);
            var criteria = new GetAnnualLeaveBalanceCriteria
            {
                Year = date.Year,
                EmpId = null,
                TeamIds = string.Join(",", teams.Select(x => x.Id)),
                IsLocal = null,
                Role = string.Empty,
                SupervisorId = null
            };
            var entityList = await reportService.GetAnnualLeaveBalanceReportAsync(criteria);
            var resetAnnualLeaveCriteria = new ResetAnnualLeaveCriteria
            {
                ActorId = EmployeeSession.EmpId,
                Description = descriptionReset,
                AnnualLeaves = entityList.Where(m => m.LeaveBalance > minBalance).ToList(),
                FromDate = date,
                ResetDays = minBalance
            };
            var result = await baseService.ExecuteNonQueryAsync(resetAnnualLeaveCriteria);

            return Json(result);
        }

        [HrmPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public async Task<IActionResult> Employee(int id)
        {
            var year = DateTimeOffset.Now.Year;
            var model = new AnnualLeaveBalanceViewModel
            {
                EmployeeId = id,
                Year = year,
                ExcludedRequestTrans = true,
                AnnualLeaveBalances = await GetAnnualLeaveTransactionAsync(id, false, year),
                Employee = await employeeService.GetEmployeeByIdAsync(id)
            };

            return View(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Read, FunctionKey.AnnualLeaveBalance)]
        public async Task<IActionResult> Employee(AnnualLeaveBalanceViewModel model)
        {
            model.Employee = await employeeService.GetEmployeeByIdAsync(model.EmployeeId);
            model.AnnualLeaveBalances = await GetAnnualLeaveTransactionAsync(model.EmployeeId, !model.ExcludedRequestTrans, model.Year);

            return View(model);
        }

        [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await employeeService.GetAnnualLeaveTransactionAsync(id);
            ViewBag.Employee = await employeeService.GetEmployeeByIdAsync(model.EmpId);
            return View(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public async Task<IActionResult> Edit(AnnualLeaveTransactionViewModel model)
        {
            var formCollection = Request.Form;
            ViewBag.Employee = await employeeService.GetEmployeeByIdAsync(model.EmpId);
            if (formCollection["command"].Equals(Labels.Delete))
            {
                return await DeleteAsync(model);
            }

            return await EditAsync(model);
        }

        [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public async Task<IActionResult> Create(int id)
        {
            ViewBag.Employee = await employeeService.GetEmployeeByIdAsync(id);
            var model = new AnnualLeaveTransactionViewModel
            {
                EmpId = id,
                AffectedFrom = DateTimeOffset.Now.DateTime,
            };
            return View(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.AnnualLeaveBalance)]
        public async Task<IActionResult> Create(AnnualLeaveTransactionViewModel model)
        {
            ViewBag.Employee = await employeeService.GetEmployeeByIdAsync(model.EmpId);
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
                var result = await baseService.ExecuteNonQueryAsync(criteria);

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

        private async Task<IEnumerable<LeaveSummary>> GetAnnualLeaveBalanceAsync(LeaveSummaryViewModel model)
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
            return await reportService.GetAnnualLeaveBalanceReportAsync(criteria);
        }

        private async Task<IEnumerable<AnnualLeaveTransactionViewModel>> GetAnnualLeaveTransactionAsync(int empId, bool excludedRequest, int year)
        {
            var criteria = new GetAnnualLeaveTransactionCriteria
            {
                EmpId = empId,
                ExcludedRequestTrans = excludedRequest,
                Year = year
            };

            return await employeeService.GetAnnualLeaveTransactionAsync(criteria);
        }

        private async Task<IActionResult> EditAsync(AnnualLeaveTransactionViewModel model)
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
                var result = await baseService.ExecuteNonQueryAsync(criteria);

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

        private async Task<IActionResult> DeleteAsync(AnnualLeaveTransactionViewModel model)
        {
            var criteria = new DeleteAnnualLeaveTransactionCriteria
            {
                LeaveTransId = model.LeaveTransId,
                ModifiedDate = DateTimeOffset.Now.DateTime,
                ModifiedBy = EmployeeSession.EmpId
            };
            var result = await baseService.ExecuteNonQueryAsync(criteria);

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