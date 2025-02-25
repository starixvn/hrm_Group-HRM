namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models.Overtime;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Overtime;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using Hangfire;

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
        public ActionResult EmployeeView(MonthlyOvertimeRequestViewModel model)
        {
            model.Employee = model.Employee ?? EmployeeSession.DisplayName;
            model.EmployeeId = model.EmployeeId <= 0 ? EmployeeSession.EmpId : model.EmployeeId;
            model.OvertimeRequests = overtimeService.GetMonthlyRequestByEmployeeId(model.EmployeeId, model.Year);

            return View(model);
        }

        public ActionResult Details(int employeeId, DateTime month)
        {
            MonthlyOvertimeRequest monthlyRequest = overtimeService.GetMonthlyRequestDetailsByEmployeeId(employeeId, month);

            return View(monthlyRequest);
        }

        public ActionResult Edit(int employeeId, DateTime month)
        {
            MonthlyOvertimeRequest monthlyRequest = overtimeService.GetMonthlyRequestDetailsByEmployeeId(employeeId, month);
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(employeeId, RequestType.Overtime);

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
        public CustomJsonResult Edit(EditMonthlyOvertimeRequest model)
        {
            CommandResult commandResult = overtimeService.EditMonthlyRequest(EmployeeSession.EmpId, model);
            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HRMPermission(Policy.Host, FunctionKey.VerifyOvertime, FunctionKey.ApproveOvertime)]
        public ActionResult MemberOvertime(MemberMonthlyOvertimeRequestViewModel model)
        {
            model.TeamIds = model.TeamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));
            model.MemberRequests = GetMemberOvertimeRequest(model);
            ViewBag.CurrentUserId = EmployeeSession.EmpId;
            ViewBag.IsVerifier = userControlSession.HasPermissionOnFunction(FunctionKey.VerifyOvertime, Policy.Host);
            ViewBag.IsApprover = userControlSession.HasPermissionOnFunction(FunctionKey.ApproveOvertime, Policy.Host);

            return View(model);
        }

        private IEnumerable<MonthlyOvertimeRequest> GetMemberOvertimeRequest(MemberMonthlyOvertimeRequestViewModel model)
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
            model.MemberRequests = overtimeService.GetMemberMonthlyRequest(param);

            return model.MemberRequests;
        }

        public ActionResult MemberDetails(long id)
        {
            MonthlyOvertimeRequest monthlyRequest = overtimeService.GetMonthlyRequestDetailsById(id);
            ViewBag.CurrentUserId = EmployeeSession.EmpId;
            return View(monthlyRequest);
        }

        [HRMPermission(Policy.Host, FunctionKey.ProcessOvertime)]
        public ActionResult EmployeeOvertime(MemberMonthlyOvertimeRequestViewModel model)
        {
            model.TeamIds = model.TeamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));

            return View(model);
        }

        public ActionResult ListEmployeeOvertime(MemberMonthlyOvertimeRequestViewModel model)
        {
            model.MemberRequests = GetEmployeeOvertimeRequest(model);

            return PartialView("_ListEmployeeOvertime", model);
        }

        private IEnumerable<MonthlyOvertimeRequest> GetEmployeeOvertimeRequest(MemberMonthlyOvertimeRequestViewModel model)
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
                IsProcessor = true,
                OvertimeType = model.OvertimeType,
                IsLocal = model.IsLocal
            };
            model.MemberRequests = overtimeService.GetMemberMonthlyRequest(param);

            return model.MemberRequests;
        }

        public ActionResult EmployeeDetails(long id)
        {
            MonthlyOvertimeRequest monthlyRequest = overtimeService.GetMonthlyRequestDetailsById(id);
            return View(monthlyRequest);
        }

        public ActionResult GetActingEmployees(string query)
        {
            var criteria = new GetEmployeeSupervisorByQueryCriteria
            {
                EmpId = EmployeeSession.EmpId,
                Role = (byte)RoleSystem.OnBehalfRequest,
                RequestType = (byte)RequestType.Overtime,
                Query = string.IsNullOrWhiteSpace(query) ? string.Empty : query.Trim(),
                IncludeLoginUser = true
            };

            IList<SupervisorNameModel> employees = employeeService.GetSupervisorByQuery(criteria);
            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.VerifyOvertime, FunctionKey.ApproveOvertime, FunctionKey.ProcessOvertime)]
        public JsonResult EditMonthlyRequestStatus(string requestIds, RequestCommand command, byte roleId)
        {
            CommandResult commandResult = overtimeService.EditMonthlyRequestStatus(EmployeeSession.EmpId, requestIds, command, roleId);
            return Json(commandResult);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.ProcessOvertime)]
        public ActionResult CheckInvalidOvertime(string requestIds)
        {
            bool isInvalid = overtimeService.CheckInvalidOvertimes(requestIds);

            return Json(isInvalid, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ProcessOvertime)]
        public void ReminderClaimOvertime(string subject, string content)
        {
            BackgroundJob.Enqueue(() => overtimeService.SendMailToReminderClaimOvertime(subject, content));
        }

        public ActionResult Export(MemberMonthlyOvertimeRequestViewModel model)
        {
            var fileName = $"{Labels.EmployeeOvertimeReport}.xlsx";
            const bool ismemberOvetime = false;
            model.MemberRequests = GetEmployeeOvertimeRequest(model);
            byte[] exportData = exportOvertimeService.Export(model.MemberRequests, ismemberOvetime);

            return File(exportData, MimeType.XLSX, fileName);
        }

        public ActionResult ExportMemberOvertime(MemberMonthlyOvertimeRequestViewModel model)
        {
            var fileName = $"{Labels.MemberOvertimeReport}.xlsx";
            const bool ismemberOvetime = true;

            model.MemberRequests = GetMemberOvertimeRequest(model);
            byte[] exportData = exportOvertimeService.Export(model.MemberRequests, ismemberOvetime);

            return File(exportData, MimeType.XLSX, fileName);
        }
    }
}