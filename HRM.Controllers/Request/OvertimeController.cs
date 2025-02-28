namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using HRM.Controllers;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Filters;
    using HRM.Core.Models;
    using HRM.Criteria.Supervisor;
    using HRM.Models.Overtime;
    using HRM.Models.Supervisor;
    using HRM.Resources;
    using HRM.Services;
    using HRM.Services.ExportExcel;
    using HRM.Services.Interfaces;
    using HRM.ViewModels.Overtime;
    using Microsoft.AspNetCore.Mvc;

    public class OvertimeController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IEmployeeService employeeService;
        private readonly IRequestService requestService;
        private readonly IOvertimeRequestService overtimeService;
        private readonly IExportOvertimeService exportOvertimeService;
        private readonly ITeamService teamService;

        public OvertimeController(
            IUserControlSession userControlSession,
            IEmployeeService employeeService,
            IRequestService requestService,
            IOvertimeRequestService overtimeService,
            IExportOvertimeService exportOvertimeService,
            ITeamService teamService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.employeeService = employeeService;
            this.requestService = requestService;
            this.overtimeService = overtimeService;
            this.exportOvertimeService = exportOvertimeService;
            this.teamService = teamService;
        }

        [HttpGet]
        public async Task<IActionResult> EmployeeView(MonthlyOvertimeRequestViewModel model)
        {
            model.Employee = model.Employee ?? EmployeeSession.DisplayName;
            model.EmployeeId = model.EmployeeId <= 0 ? EmployeeSession.EmpId : model.EmployeeId;
            model.OvertimeRequests = await overtimeService.GetMonthlyRequestByEmployeeId(model.EmployeeId, model.Year);

            return View(model);
        }

        public async Task<IActionResult> Details(int employeeId, DateTime month)
        {
            var monthlyRequest = await overtimeService.GetMonthlyRequestDetailsByEmployeeId(employeeId, month);

            return View(monthlyRequest);
        }

        public async Task<IActionResult> Edit(int employeeId, DateTime month)
        {
            var monthlyRequest = await overtimeService.GetMonthlyRequestDetailsByEmployeeId(employeeId, month);
            var supervisor = await requestService.GetVerifierAndApproverAsync(employeeId, RequestType.Overtime);

            var model = new EditMonthlyRequestViewModel
            {
                MonthlyRequest = monthlyRequest,
                VerifierId = monthlyRequest.Verifier != null ? monthlyRequest.Verifier.EmpId : 0,
                ApproverId = monthlyRequest.Approver != null ? monthlyRequest.Approver.EmpId : 0,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditMonthlyOvertimeRequest model)
        {
            var commandResult = await overtimeService.EditMonthlyRequest(EmployeeSession.EmpId, model);
            return Json(commandResult);
        }

        [HrmPermission(Policy.Host, FunctionKey.VerifyOvertime, FunctionKey.ApproveOvertime)]
        public async Task<IActionResult> MemberOvertime(MemberMonthlyOvertimeRequestViewModel model)
        {
            var teams = await teamService.GetAllAsync();
            model.TeamIds = model.TeamIds ?? string.Join(",", teams.Select(x => x.Id));
            model.MemberRequests = await GetMemberOvertimeRequest(model);
            ViewBag.CurrentUserId = EmployeeSession.EmpId;
            ViewBag.IsVerifier = userControlSession.HasPermissionOnFunction(FunctionKey.VerifyOvertime, Policy.Host);
            ViewBag.IsApprover = userControlSession.HasPermissionOnFunction(FunctionKey.ApproveOvertime, Policy.Host);

            return View(model);
        }

        private async Task<IEnumerable<MonthlyOvertimeRequest>> GetMemberOvertimeRequest(MemberMonthlyOvertimeRequestViewModel model)
        {
            var statusIds = string.Empty;
            IEnumerable<int> requestStatusIds = Enum.GetValues(typeof(CurrentRequestStatus)).Cast<int>().Where(x => x > 0);
            if (model.StatusId == -1)
            {
                statusIds = string.Format("{0},{1}", (byte)CurrentRequestStatus.Pending, (byte)CurrentRequestStatus.Verified);
            }
            else if (model.StatusId == 0)
            {
                statusIds = string.Join(",", requestStatusIds);
            }
            else
            {
                statusIds = model.StatusId.ToString();
            }

            var param = new GetMonthlyOvertimeRequestParam
            {
                ActorId = EmployeeSession.EmpId,
                Month = model.Month,
                TeamIds = model.TeamIds,
                EmployeeId = model.EmployeeId,
                StatusIds = statusIds,
                IsLocal = model.IsLocal
            };
            model.MemberRequests = await overtimeService.GetMemberMonthlyRequest(param);

            return model.MemberRequests;
        }

        public async Task<IActionResult> MemberDetails(long id)
        {
            var monthlyRequest = await overtimeService.GetMonthlyRequestDetailsById(id);
            ViewBag.CurrentUserId = EmployeeSession.EmpId;
            return View(monthlyRequest);
        }

        [HrmPermission(Policy.Host, FunctionKey.ProcessOvertime)]
        public async Task<IActionResult> EmployeeOvertime(MemberMonthlyOvertimeRequestViewModel model)
        {
            var teams = await teamService.GetAllAsync();
            model.TeamIds = model.TeamIds ?? string.Join(",", teams.Select(x => x.Id));

            return View(model);
        }

        public async Task<IActionResult> ListEmployeeOvertime(MemberMonthlyOvertimeRequestViewModel model)
        {
            model.MemberRequests = await GetEmployeeOvertimeRequest(model);

            return PartialView("_ListEmployeeOvertime", model);
        }

        private async Task<IEnumerable<MonthlyOvertimeRequest>> GetEmployeeOvertimeRequest(MemberMonthlyOvertimeRequestViewModel model)
        {
            var statusIds = string.Empty;
            var requestStatusIds = Enum.GetValues(typeof(CurrentRequestStatus)).Cast<int>().Where(x => x > 0);
            if (model.StatusId == -1)
            {
                statusIds = string.Format("{0},{1}", (byte)CurrentRequestStatus.Pending, (byte)CurrentRequestStatus.Verified);
            }
            else if (model.StatusId == 0)
            {
                statusIds = string.Join(",", requestStatusIds);
            }
            else
            {
                statusIds = model.StatusId.ToString();
            }

            var param = new GetMonthlyOvertimeRequestParam
            {
                ActorId = EmployeeSession.EmpId,
                Month = model.Month,
                TeamIds = model.TeamIds,
                EmployeeId = model.EmployeeId,
                StatusIds = statusIds,
                IsProcessor = true,
                OvertimeType = model.OvertimeType,
                IsLocal = model.IsLocal
            };
            model.MemberRequests = await overtimeService.GetMemberMonthlyRequest(param);

            return model.MemberRequests;
        }

        public async Task<IActionResult> EmployeeDetails(long id)
        {
            var monthlyRequest = await overtimeService.GetMonthlyRequestDetailsById(id);
            return View(monthlyRequest);
        }

        public async Task<IActionResult> GetActingEmployees(string query)
        {
            var criteria = new GetEmployeeSupervisorByQueryCriteria
            {
                EmpId = EmployeeSession.EmpId,
                Role = (byte)RoleSystem.OnBehalfRequest,
                RequestType = (byte)RequestType.Overtime,
                Query = string.IsNullOrWhiteSpace(query) ? string.Empty : query.Trim(),
                IncludeLoginUser = true
            };

            var employees = await employeeService.GetSupervisorByQueryAsync(criteria);
            return Json(employees);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.VerifyOvertime, FunctionKey.ApproveOvertime, FunctionKey.ProcessOvertime)]
        public async Task<IActionResult> EditMonthlyRequestStatus(string requestIds, RequestCommand command, byte roleId)
        {
            var commandResult = await overtimeService.EditMonthlyRequestStatus(EmployeeSession.EmpId, requestIds, command, roleId);
            return Json(commandResult);
        }

        [HttpPost]
        [HrmPermission(Policy.Read, FunctionKey.ProcessOvertime)]
        public async Task<IActionResult> CheckInvalidOvertime(string requestIds)
        {
            bool isInvalid = await overtimeService.CheckInvalidOvertimes(requestIds);

            return Json(isInvalid);
        }

        //[HttpPost]
        //[HrmPermission(Policy.Host, FunctionKey.ProcessOvertime)]
        //public async Task<IActionResult> ReminderClaimOvertime(string subject, string content)
        //{
        //    BackgroundJob.Enqueue(() => overtimeService.SendMailToReminderClaimOvertime(subject, content));
        //}

        public async Task<IActionResult> Export(MemberMonthlyOvertimeRequestViewModel model)
        {
            var fileName = $"{Labels.EmployeeOvertimeReport}.xlsx";
            const bool ismemberOvetime = false;
            model.MemberRequests = await GetEmployeeOvertimeRequest(model);
            byte[] exportData = exportOvertimeService.Export(model.MemberRequests, ismemberOvetime);

            return File(exportData, MimeType.XLSX, fileName);
        }

        public async Task<IActionResult> ExportMemberOvertime(MemberMonthlyOvertimeRequestViewModel model)
        {
            var fileName = $"{Labels.MemberOvertimeReport}.xlsx";
            const bool ismemberOvetime = true;

            model.MemberRequests = await GetMemberOvertimeRequest(model);
            byte[] exportData = exportOvertimeService.Export(model.MemberRequests, ismemberOvetime);

            return File(exportData, MimeType.XLSX, fileName);
        }
    }
}