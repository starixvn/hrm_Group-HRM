using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Filters;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.SickLeave;
    using HRM.Models.HospitalizationLeave;
    using HRM.Resources;
    using HRM.Services;
    using HRM.ViewModels.AnnualLeave;
    using HRM.ViewModels.HospitalizationLeave;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using NPOI.HSSF.UserModel;

    [HrmPermission(Policy.Read, FunctionKey.SickLeaveBalance)]
    public class SickLeaveBalanceController : BaseExportController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IEmployeeService employeeService;
        private readonly ITeamService teamService;
        private readonly IBaseService baseService;
        private readonly IExportExcelService exportExcelService;

        public SickLeaveBalanceController(
            IUserControlSession userControlSession,
            IEmployeeService employeeService,
            ITeamService teamService,
            IBaseService baseService,
            IExportExcelService exportExcelService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.employeeService = employeeService;
            this.teamService = teamService;
            this.baseService = baseService;
            this.exportExcelService = exportExcelService;
        }

        public ActionResult Index(
            string teamIds,
            string employee,
            int? year = null,
            int? employeeId = null,
            bool? isLocal = null)
        {
            var model = new HospitalizationLeaveBalanceViewModel
            {
                Year = year ?? DateTimeOffset.Now.Year,
                TeamIds = teamIds,
                Employee = employee,
                EmployeeId = employeeId,
                IsLocal = isLocal,
                IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.SickLeaveBalance, Policy.Host)
            };

            return Index(model);
        }

        [HttpPost]
        public ActionResult Index(HospitalizationLeaveBalanceViewModel model)
        {
            model.Teams = teamService.GetBySupervisor(EmployeeSession.EmpId);
            model.TeamIds = !string.IsNullOrWhiteSpace(model.TeamIds) ? model.TeamIds : string.Join(",", model.Teams.Select(x => x.Id));
            var criteria = new GetSummaryReportCriteria
            {
                Year = model.Year,
                EmployeeId = model.EmployeeId,
                TeamIds = model.TeamIds,
                IsLocal = model.IsLocal,
                SupervisorId = EmployeeSession.EmpId
            };
            model.Employees = baseService.GetData<HospitalizationLeaveBalanceSummaryReport>(criteria);

            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult MultipleInsert()
        {
            var model = new AnnualLeaveBalanceMultiViewModel
            {
                AffectedFrom = DateTimeOffset.Now.Date
            };
            return PartialView("MultipleInsert", model);
        }

        [ChildActionOnly]
        public PartialViewResult MultipleReset()
        {
            var model = new AnnualLeaveBalanceMultiViewModel
            {
                AffectedFrom = DateTimeOffset.Now.Date
            };
            return PartialView(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [HrmPermission(Policy.Host, FunctionKey.SickLeaveBalance)]
        public JsonResult Adjust(AnnualLeaveBalanceMultiViewModel model)
        {
            var criteria = new InsertMultipleTransactionCriteria
            {
                EmployeeIds = model.EmpIds,
                Description = model.Description,
                CreditDays = model.CreditDays,
                DeductDays = model.DeductDays,
                FromDate = model.AffectedFrom,
                ActorId = EmployeeSession.EmpId
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [ValidateInput(false)]
        [HrmPermission(Policy.Host, FunctionKey.SickLeaveBalance)]
        public JsonResult Reset(ResetMultipleTransactionCriteria criteria)
        {
            criteria.ActorId = EmployeeSession.EmpId;
            criteria.Year = DateTimeOffset.Now.AddYears(-1).Year;
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HrmPermission(Policy.Host, FunctionKey.SickLeaveBalance)]
        public ActionResult Employee(int id)
        {
            var employee = employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            var year = DateTimeOffset.Now.Year;
            var model = new HospitalizationLeaveTransactionViewModel
            {
                EmployeeId = id,
                Nickname = employee.NickName,
                Year = year,
                IncludedRequestTrans = false,
                Transactions = GetSickLeaveTransaction(id, year, false)
            };

            return View(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.SickLeaveBalance)]
        public ActionResult Employee(HospitalizationLeaveTransactionViewModel model)
        {
            var employee = employeeService.GetEmployeeById(model.EmployeeId);
            model.Nickname = employee.NickName;
            model.Transactions = GetSickLeaveTransaction(model.EmployeeId, model.Year, model.IncludedRequestTrans);

            return View(model);
        }

        [HrmPermission(Policy.Host, FunctionKey.SickLeaveBalance)]
        public ActionResult Create(int id)
        {
            var employee = employeeService.GetEmployeeById(id);
            var model = new HospitalizationLeaveTransaction
            {
                EmpId = id,
                Nickname = employee.NickName,
                AffectedFrom = DateTimeOffset.Now.Date,
            };
            return View(model);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.SickLeaveBalance)]
        public ActionResult Create(HospitalizationLeaveTransaction model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new InsertTransactionCriteria
                {
                    EmpId = model.EmpId,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.DeductDays,
                    CreditDays = model.CreditDays,
                    CreatedDate = DateTimeOffset.Now.DateTime,
                    CreatedBy = EmployeeSession.EmpId
                };
                CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

                if (commandResult.IsSuccessful)
                {
                    return RedirectToAction("Employee", new { id = model.EmpId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, commandResult.Message);
                }
            }

            return View(model);
        }

        [HrmPermission(Policy.Host, FunctionKey.SickLeaveBalance)]
        public ActionResult Edit(int id)
        {
            var criteria = new GetTransactionByIdCriteria
            {
                TransId = id
            };
            var transaction = baseService.GetDataEntity<HospitalizationLeaveTransaction>(criteria);
            if (transaction == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return View(transaction);
        }

        [HttpPost]
        [ValidateInput(false)]
        [HrmPermission(Policy.Host, FunctionKey.SickLeaveBalance)]
        public ActionResult Edit(FormCollection formCollection, HospitalizationLeaveTransaction model)
        {
            if (formCollection["command"].Equals(Labels.Delete))
            {
                return Delete(model);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    return Edit(model);
                }
                else
                {
                    return View(model);
                }
            }
        }

        public ActionResult ExportSummaryReport(HospitalizationLeaveBalanceViewModel model)
        {
            string fileName = "SickLeaveSummary_" + DateTimeOffset.Now.ToString("yyyyMMdd") + ".xls";
            var criteria = new GetSummaryReportCriteria
            {
                Year = model.Year,
                EmployeeId = model.EmployeeId,
                TeamIds = model.TeamIds,
                IsLocal = model.IsLocal,
                SupervisorId = EmployeeSession.EmpId
            };
            var employees = baseService.GetData<HospitalizationLeaveBalanceSummaryReport>(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportSickLeaveSummary(employees, model.Year);

            return ExcelFile(hssfworkbook, fileName);
        }

        private IEnumerable<HospitalizationLeaveTransaction> GetSickLeaveTransaction(int employeeId, int year, bool includedRequestTrans = true)
        {
            var criteria = new GetTransactionByEmployeeIdCriteria
            {
                EmployeeId = employeeId,
                IncludedRequestTrans = includedRequestTrans,
                Year = year
            };

            return baseService.GetData<HospitalizationLeaveTransaction>(criteria);
        }

        private ActionResult Edit(HospitalizationLeaveTransaction model)
        {
            var criteria = new UpdateTransactionCriteria
            {
                TransId = model.TransId,
                Description = model.Description,
                FromDate = model.AffectedFrom,
                DeductDays = model.DeductDays,
                CreditDays = model.CreditDays,
                ModifiedDate = DateTimeOffset.Now.DateTime,
                ModifiedBy = EmployeeSession.EmpId
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                ViewBag.Updated = true;
            }
            else
            {
                ModelState.AddModelError(string.Empty, commandResult.Message);
            }

            return View(model);
        }

        private ActionResult Delete(HospitalizationLeaveTransaction model)
        {
            var criteria = new DeleteTransactionCriteria
            {
                TransId = model.TransId,
                ModifiedDate = DateTimeOffset.Now.DateTime,
                ModifiedBy = EmployeeSession.EmpId
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                return RedirectToAction("Employee", new { id = model.EmpId });
            }
            else
            {
                ModelState.AddModelError(string.Empty, commandResult.Message);
                return View(model);
            }
        }
    }
}