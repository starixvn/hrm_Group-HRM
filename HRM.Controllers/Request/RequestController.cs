using System.Threading.Tasks;
using HRM.Services.Interfaces;
using HRM.Services.Interfaces.Request;

namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Filters;
    using HRM.Core.Models;
    using HRM.Criteria.Request;
    using HRM.Criteria.Supervisor;
    using HRM.Models;
    using HRM.Models.Request;
    using HRM.Models.Supervisor;
    using HRM.Services;
    using HRM.ViewModels.Request;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using HRM.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class RequestController(
        IUserControlSession userControlSession,
        IEmployeeService employeeService,
        IRequestService requestService,
        ICommonDataService commonDataService,
        ITeamService teamService,
        IAttachFileService attachFileService)
        : BaseController(userControlSession)
    {
        private const string PathFolder = "/file/uploads/request/";
        private const string EmployeeRequestViewName = "EmployeeRequest";
        private readonly IRequestService requestService = requestService;
        private readonly ICommonDataService commonDataService = commonDataService;
        private readonly ITeamService teamService = teamService;
        private readonly IAttachFileService attachFileService = attachFileService;

        //[HttpPost]
        //public string GetMessageDetailRequest(RequestEntity request, short requestType, string fromShiftCode, string toShiftCode)
        //{
        //    request.RequestType = new RequestTypeModel { Id = requestType };
        //    request.FromShift = new Shift { Name = fromShiftCode };
        //    request.ToShift = new Shift { Name = toShiftCode };

        //    var date = RequestService.ToRequestMessage(request);
        //    return date;
        //}

        //[HRMPermission(Policy.Host, FunctionKey.VerifyRequest, FunctionKey.ApproveRequest)]
        //public ActionResult Member(RequestViewModel request)
        //{
        //    request.ViewType = RequestViewType.PersonalSpace;
        //    request.TeamIds = request.TeamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));

        //    if (request.StatusId == Constants.DefaultRequestFilterStatusId)
        //    {
        //        const long FromDays = -180;
        //        request.StatusId = Constants.PendingAndVerifiedStatusId;
        //        request.FromDate = DateTimeOffset.Now.Date.AddDays(FromDays);
        //        request.IsFilterByAffectedDate = false;
        //    }
        //    GetRequests(request);
        //    GetInfoAttachFile(request.Requests);

        //    return View(EmployeeRequestViewName, request);
        //}

        private void GetInfoAttachFile(IList<RequestEntity> requests)
        {
            foreach (var request in requests)
            {
                request.AttachFiles = attachFileService.GetInfoAttachFile(request.Id, PathFolder);
            }
        }

        //[HRMPermission(Policy.Host, FunctionKey.ProcessRequest)]
        //public ActionResult HRView(RequestViewModel request)
        //{
        //    request.StatusId = request.StatusId == Constants.DefaultRequestFilterStatusId ? 0 : request.StatusId;
        //    request.ViewType = RequestViewType.HRSpace;
        //    request.TeamIds = request.TeamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));
        //    GetRequests(request);
        //    GetInfoAttachFile(request.Requests);

        //    return View(EmployeeRequestViewName, request);
        //}

        //[HRMPermission(Policy.Read, FunctionKey.DailyRequest)]
        //public ActionResult DailyView(RequestViewModel request)
        //{
        //    request.StatusId = request.StatusId == Constants.DefaultRequestFilterStatusId ? 0 : request.StatusId;
        //    request.ViewType = RequestViewType.CompanySpace;
        //    request.TeamIds = request.TeamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));

        //    GetRequests(request);

        //    return View(EmployeeRequestViewName, request);
        //}

        [HrmPermission(Policy.Host, FunctionKey.VoidRequest)]
        public async Task<ActionResult> VoidRequest(RequestViewModel request)
        {
            request.StatusId = request.StatusId == Constants.DefaultRequestFilterStatusId ? 0 : request.StatusId;
            request.ViewType = RequestViewType.SASpace;
            var teams = await teamService.GetAllAsync();
            request.TeamIds ??= string.Join(",", teams.Select(x => x.Id));
            await GetRequests(request);
            GetInfoAttachFile(request.Requests);

            return View(EmployeeRequestViewName, request);
        }

        public async Task<IActionResult> GetEmployeesByActingUser(byte role, string query = "", byte? requestType = null)
        {
            var criteria = new GetEmployeeSupervisorByQueryCriteria
            {
                EmpId = EmployeeSession.EmpId,
                Role = role,
                RequestType = requestType,
                Query = query.Trim(),
                IncludeLoginUser = true
            };
            var supervisors = await employeeService.GetSupervisorByQueryAsync(criteria);

            return Json(supervisors);
        }

        public ActionResult GetVerifiers(int employeeId, byte requestType)
        {
            var verifierList = GetSupervisorByRequestType(employeeId, (byte)RoleSystem.Verifier, requestType);

            return Json(verifierList);
        }

        public ActionResult GetApprovers(int employeeId, byte requestType)
        {
            var approverList = GetSupervisorByRequestType(employeeId, (byte)RoleSystem.Approver, requestType);

            return Json(approverList);
        }

        [NonAction]
        public IList<SupervisorNameModel> GetSupervisorByRequestType(int employeeId, byte role, byte requestType)
        {
            var criteria = new GetSupervisorByRequestTypeCriteria
            {
                EmpId = employeeId,
                Role = role,
                RequestType = requestType
            };
            var supervisors = employeeService.GetSupervisorByRequestType(criteria);

            return supervisors;
        }

        [HrmPermission(Policy.Host, FunctionKey.VerifyRequest, FunctionKey.ApproveRequest, FunctionKey.ProcessRequest, FunctionKey.VoidRequest)]
        public PartialViewResult PartialViewProcessRequest(ProcessRequestViewModel model)
        {
            if (model != null
                && model.RequestId > 0
                && model.RoleId == (byte)RoleSystem.Approver
                && model.Command == RequestCommand.Approve)
            {
                HandleProcessRequest(model);
            }

            return PartialView("_ProcessRequest", model);
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.VerifyRequest, FunctionKey.ApproveRequest, FunctionKey.ProcessRequest, FunctionKey.VoidRequest)]
        public async Task<CustomJsonResult> ProcessRequest(ProcessRequestParam model)
        {
            CommandResult commandResult;
            switch (model.Command)
            {
                case RequestCommand.Accept:
                case RequestCommand.Approve:
                case RequestCommand.Process:
                    commandResult = await requestService.ApproveRequest(EmployeeSession.EmpId, model);
                    break;

                case RequestCommand.Reset:
                    commandResult = await requestService.ResetRequest(EmployeeSession.EmpId, model);
                    break;

                case RequestCommand.NotAccept:
                case RequestCommand.Reject:
                case RequestCommand.Void:
                    commandResult = await requestService.RejectRequest(EmployeeSession.EmpId, model);
                    break;

                default:
                    commandResult = await requestService.VoidRequest(EmployeeSession.EmpId, model);
                    break;
            }

            if (commandResult.IsSuccessful)
            {
                 await requestService.SendMailProcessRequest(model, EmployeeSession, model.Command);
            }

            return new CustomJsonResult(commandResult);
        }

        //[HttpPost]
        //[ValidateInput(false)]
        //[HRMPermission(Policy.Host, FunctionKey.ApproveRequest)]
        //public CustomJsonResult ProcessMultipleRequests(ApproveRequestsParam model)
        //{
        //    model.ActorId = EmployeeSession.EmpId;

        //    var commandResult = requestService.ApproveRequests(model);

        //    if (commandResult.IsSuccessful)
        //    {
        //        requestService.SendMailProcessRequests(model, EmployeeSession, model.Command);
        //    }

        //    return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

        private async Task HandleProcessRequest(ProcessRequestViewModel model)
        {
            switch (model.RequestTypeId)
            {
                case (byte)RequestType.CheckIn:
                case (byte)RequestType.CheckOut:
                    model.CheckInOutDetail = await requestService.GetCheckInOutDetailToReviewDiscipline(model.RequestId);
                    break;

                case (byte)RequestType.LateIn:
                case (byte)RequestType.EarlyOut:
                    model.LateInEarlyOutDetail = await requestService.GetLateInEarlyOutDetailToReviewDiscipline(model.RequestId);
                    break;

                case (byte)RequestType.ChangeShift:
                    model.ChangeShiftDetails = await requestService.GetChangeShiftDetailToReviewDiscipline(model.RequestId);
                    break;

                case (byte)RequestType.Leave:
                    model.LeaveDisciplines = await requestService.GetLeaveRequestDetailToReviewDiscipline(model.RequestId);
                    break;
            }
        }

        [NonAction]
        private async Task GetRequests(RequestViewModel request)
        {
            request.ToDate = request.ToDate.ToEndOfDay();
            request.LeaveTypes = await commonDataService.GetLeaveTypeAsync();
            int? employeeId = null;
            if (request.EmployeeId != -1 && !string.IsNullOrWhiteSpace(request.Employee))
            {
                employeeId = request.EmployeeId;
            }

            bool? isLocal = null;
            var criteria = new GetRequestCriteria
            {
                EmpId = employeeId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                TeamIds = request.TeamIds,
                Status = request.StatusIds,
                RequestType = request.RequestType,
                IsQueryByCreatedDate = !request.IsFilterByAffectedDate,
                RequestViewType = request.ViewType,
                SupervisorId = EmployeeSession.EmpId,
                IsLocal = request.IsLocal.HasValue ? (request.IsLocal.Value == 1) : isLocal,
                LeaveTypeId = request.LeaveType
            };

            var requestList = await requestService.GetRequests(criteria);
            request.CurrentUserId = EmployeeSession.EmpId;
            request.Requests = request.ViewType == RequestViewType.PersonalSpace ? SortRequest(requestList) : requestList;
        }

        private IList<RequestEntity> SortRequest(IList<RequestEntity> requests)
        {
            return requests.OrderByDescending(t => (SortRequestValidate(t))).ToList();
        }

        private bool SortRequestValidate(RequestEntity t)
        {
            var pending = t.Status.Id == (byte)CurrentRequestStatus.Pending
                && (t.ApprovedBy.Id == EmployeeSession.EmpId || t.VerifiedBy.Id == EmployeeSession.EmpId);
            var verified = (t.Status.Id == (byte)CurrentRequestStatus.Verified && t.ApprovedBy.Id == EmployeeSession.EmpId);
            return pending || verified;
        }
    }
}