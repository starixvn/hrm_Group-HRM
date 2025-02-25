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
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class RequestController : BaseController
    {
        private const string PathFolder = "/file/uploads/request/";
        private const string EmployeeRequestViewName = "EmployeeRequest";
        private readonly IEmployeeService employeeService;
        private readonly IRequestService requestService;
        private readonly ICommonDataService commonDataService;
        private readonly ITeamService teamService;
        private readonly IAttachFileService attachFileService;

        public RequestController(
            IUserControlSession userControlSession,
            IEmployeeService employeeService,
            IRequestService requestService,
            ICommonDataService commonDataService,
            ITeamService teamService,
            IAttachFileService attachFileService) : base(userControlSession)
        {
            this.employeeService = employeeService;
            this.requestService = requestService;
            this.commonDataService = commonDataService;
            this.teamService = teamService;
            this.attachFileService = attachFileService;
        }

        [HttpPost]
        public string GetMessageDetailRequest(RequestEntity request, short requestType, string fromShiftCode, string toShiftCode)
        {
            request.RequestType = new RequestTypeModel { Id = requestType };
            request.FromShift = new Shift { Name = fromShiftCode };
            request.ToShift = new Shift { Name = toShiftCode };

            var date = RequestService.ToRequestMessage(request);
            return date;
        }

        [HRMPermission(Policy.Host, FunctionKey.VerifyRequest, FunctionKey.ApproveRequest)]
        public ActionResult Member(RequestViewModel request)
        {
            request.ViewType = RequestViewType.PersonalSpace;
            request.TeamIds = request.TeamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));

            if (request.StatusId == Constants.DefaultRequestFilterStatusId)
            {
                const long FromDays = -180;
                request.StatusId = Constants.PendingAndVerifiedStatusId;
                request.FromDate = DateTimeOffset.Now.Date.AddDays(FromDays);
                request.IsFilterByAffectedDate = false;
            }
            GetRequests(request);
            GetInfoAttachFile(request.Requests);

            return View(EmployeeRequestViewName, request);
        }

        private void GetInfoAttachFile(IList<RequestEntity> requests)
        {
            foreach (var request in requests)
            {
                request.AttachFiles = attachFileService.GetInfoAttachFile(request.Id, PathFolder);
            }
        }

        [HRMPermission(Policy.Host, FunctionKey.ProcessRequest)]
        public ActionResult HRView(RequestViewModel request)
        {
            request.StatusId = request.StatusId == Constants.DefaultRequestFilterStatusId ? 0 : request.StatusId;
            request.ViewType = RequestViewType.HRSpace;
            request.TeamIds = request.TeamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));
            GetRequests(request);
            GetInfoAttachFile(request.Requests);

            return View(EmployeeRequestViewName, request);
        }

        [HRMPermission(Policy.Read, FunctionKey.DailyRequest)]
        public ActionResult DailyView(RequestViewModel request)
        {
            request.StatusId = request.StatusId == Constants.DefaultRequestFilterStatusId ? 0 : request.StatusId;
            request.ViewType = RequestViewType.CompanySpace;
            request.TeamIds = request.TeamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));

            GetRequests(request);

            return View(EmployeeRequestViewName, request);
        }

        [HRMPermission(Policy.Host, FunctionKey.VoidRequest)]
        public ActionResult VoidRequest(RequestViewModel request)
        {
            request.StatusId = request.StatusId == Constants.DefaultRequestFilterStatusId ? 0 : request.StatusId;
            request.ViewType = RequestViewType.SASpace;
            request.TeamIds = request.TeamIds ?? string.Join(",", teamService.GetAll().Select(x => x.Id));
            GetRequests(request);
            GetInfoAttachFile(request.Requests);

            return View(EmployeeRequestViewName, request);
        }

        public JsonResult GetEmployeesByActingUser(byte role, string query = "", byte? requestType = null)
        {
            var criteria = new GetEmployeeSupervisorByQueryCriteria
            {
                EmpId = EmployeeSession.EmpId,
                Role = role,
                RequestType = requestType,
                Query = query.Trim(),
                IncludeLoginUser = true
            };
            IList<SupervisorNameModel> supervisors = employeeService.GetSupervisorByQuery(criteria);

            return Json(supervisors, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVerifiers(int employeeId, byte requestType)
        {
            IList<SupervisorNameModel> verifierList = GetSupervisorByRequestType(employeeId, (byte)RoleSystem.Verifier, requestType);

            return Json(verifierList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetApprovers(int employeeId, byte requestType)
        {
            IList<SupervisorNameModel> approverList = GetSupervisorByRequestType(employeeId, (byte)RoleSystem.Approver, requestType);

            return Json(approverList, JsonRequestBehavior.AllowGet);
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
            IList<SupervisorNameModel> supervisors = employeeService.GetSupervisorByRequestType(criteria);

            return supervisors;
        }

        [HRMPermission(Policy.Host, FunctionKey.VerifyRequest, FunctionKey.ApproveRequest, FunctionKey.ProcessRequest, FunctionKey.VoidRequest)]
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
        [ValidateInput(false)]
        [HRMPermission(Policy.Host, FunctionKey.VerifyRequest, FunctionKey.ApproveRequest, FunctionKey.ProcessRequest, FunctionKey.VoidRequest)]
        public CustomJsonResult ProcessRequest(ProcessRequestParam model)
        {
            CommandResult commandResult;
            switch (model.Command)
            {
                case RequestCommand.Accept:
                case RequestCommand.Approve:
                case RequestCommand.Process:
                    commandResult = requestService.ApproveRequest(EmployeeSession.EmpId, model);
                    break;

                case RequestCommand.Reset:
                    commandResult = requestService.ResetRequest(EmployeeSession.EmpId, model);
                    break;

                case RequestCommand.NotAccept:
                case RequestCommand.Reject:
                case RequestCommand.Void:
                    commandResult = requestService.RejectRequest(EmployeeSession.EmpId, model);
                    break;

                default:
                    commandResult = requestService.VoidRequest(EmployeeSession.EmpId, model);
                    break;
            }

            if (commandResult.IsSuccessful)
            {
                requestService.SendMailProcessRequest(model, EmployeeSession, model.Command);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [ValidateInput(false)]
        [HRMPermission(Policy.Host, FunctionKey.ApproveRequest)]
        public CustomJsonResult ProcessMultipleRequests(ApproveRequestsParam model)
        {
            model.ActorId = EmployeeSession.EmpId;

            var commandResult = requestService.ApproveRequests(model);

            if (commandResult.IsSuccessful)
            {
                requestService.SendMailProcessRequests(model, EmployeeSession, model.Command);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private void HandleProcessRequest(ProcessRequestViewModel model)
        {
            switch (model.RequestTypeId)
            {
                case (byte)RequestType.CheckIn:
                case (byte)RequestType.CheckOut:
                    model.CheckInOutDetail = requestService.GetCheckInOutDetailToReviewDiscipline(model.RequestId);
                    break;

                case (byte)RequestType.LateIn:
                case (byte)RequestType.EarlyOut:
                    model.LateInEarlyOutDetail = requestService.GetLateInEarlyOutDetailToReviewDiscipline(model.RequestId);
                    break;

                case (byte)RequestType.ChangeShift:
                    model.ChangeShiftDetails = requestService.GetChangeShiftDetailToReviewDiscipline(model.RequestId);
                    break;

                case (byte)RequestType.Leave:
                    model.LeaveDisciplines = requestService.GetLeaveRequestDetailToReviewDiscipline(model.RequestId);
                    break;

                default:
                    // nothing
                    break;
            }
        }

        [NonAction]
        private void GetRequests(RequestViewModel request)
        {
            request.ToDate = request.ToDate.ToEndOfDay();
            request.LeaveTypes = commonDataService.GetLeaveType();
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

            IList<RequestEntity> requestList = requestService.GetRequests(criteria);
            request.CurrentUserId = EmployeeSession.EmpId;
            request.Requests = request.ViewType == RequestViewType.PersonalSpace ? SortRequest(requestList) : requestList;
        }

        private IList<RequestEntity> SortRequest(IList<RequestEntity> requests)
        {
            return requests.OrderByDescending(t => (SortRequestValidate(t))).ToList();
        }

        private bool SortRequestValidate(RequestEntity t)
        {
            bool pending = t.Status.Id == (byte)CurrentRequestStatus.Pending
                && (t.ApprovedBy.Id == EmployeeSession.EmpId || t.VerifiedBy.Id == EmployeeSession.EmpId);
            bool verified = (t.Status.Id == (byte)CurrentRequestStatus.Verified && t.ApprovedBy.Id == EmployeeSession.EmpId);
            return pending || verified;
        }
    }
}