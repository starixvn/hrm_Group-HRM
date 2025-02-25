namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class LateInEarlyOutController : BaseController
    {
        private const string UploadFolderPath = "/file/uploads/request/";
        private const int DefaultMinutes = 60;
        private readonly IUserControlSession userControlSession;
        private readonly ILateInEarlyOutRequestService lateInEarlyOutRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;
        private readonly IAttachFileService attachFileService;

        public LateInEarlyOutController(
            IUserControlSession userControlSession,
            ILateInEarlyOutRequestService lateInEarlyOutRequestService,
            IRequestService requestService,
            IBaseService baseService,
            IAttachFileService attachFileService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.lateInEarlyOutRequestService = lateInEarlyOutRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
            this.attachFileService = attachFileService;
        }

        [HttpGet]
        public ActionResult EmployeeView(int? year)
        {
            int tempYear = year ?? DateTimeOffset.Now.Year;
            ViewBag.Year = tempYear;
            var fromDate = new DateTimeOffset(tempYear, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
            var criteria = new GetLateInEarlyOutRequestByDateCriteria
            {
                EmpId = EmployeeSession.EmpId,
                FromDate = fromDate,
                ToDate = fromDate.AddYears(1).AddDays(-1)
            };
            IList<LateInEarlyOutRequest> requests = lateInEarlyOutRequestService.GetByDate(criteria);
            foreach (var request in requests)
            {
                request.AttachFiles = attachFileService.GetInfoAttachFile(request.RequestId, UploadFolderPath);
            }
            return View(requests);
        }

        public ActionResult Create()
        {
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(EmployeeSession.EmpId, RequestType.LateIn);
            var viewModel = new LateInEarlyOutRequestViewModel
            {
                Employee = EmployeeSession.DisplayName,
                EmployeeId = EmployeeSession.EmpId,
                OnDate = DateTimeOffset.Now.Date,
                Minute = DefaultMinutes,
                ReasonId = (byte)LateInEarlyOutReason.Other,
                RequestType = (byte)RequestType.LateIn,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers
            };
            ViewBag.IsTopManager = EmployeeSession.IsTopManager;
            ViewBag.HasOnBehalfRequestPermission = userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

            return View(viewModel);
        }

        [HttpPost]
        public CustomJsonResult Create(string requestModel, IList<HttpPostedFileBase> files)
        {
            var model = JsonConverter.FromJson<LateInEarlyOutRequestViewModel>(requestModel);
            var criteria = new InsertLateInEarlyOutRequestCriteria
            {
                EmpId = model.EmployeeId,
                OnDate = model.OnDate,
                RequestType = model.RequestType,
                Remark = model.Remark,
                LateInMins = model.RequestType == (byte)RequestType.LateIn ? model.Minute : 0,
                EarlyOutMins = model.RequestType == (byte)RequestType.EarlyOut ? model.Minute : 0,
                ReasonId = model.ReasonId,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                CreatedBy = EmployeeSession.EmpId
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                lateInEarlyOutRequestService.SendMailAndSendLetstalkCreateRequest(criteria.RequestId);
                attachFileService.UploadFile(files, criteria.RequestId, UploadFolderPath);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Edit(int id)
        {
            LateInEarlyOutRequest request = lateInEarlyOutRequestService.GetById(id);
            if (request == null
                || !Utilities.CheckUserCanEditRequest(EmployeeSession.EmpId, request.Requester.EmpId)
                || request.Status.Id != (byte)CurrentRequestStatus.Pending)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(request.Requester.EmpId, RequestType.LateIn);
            var minutes = request.RequestTypeId == (byte)RequestType.LateIn ? request.LateInMins : request.EarlyOutMins;
            var param = new GetDisabledDateParam
            {
                EmployeeId = request.Requester.EmpId,
                FromDate = request.OnDate.AddMonths(-1),
                ToDate = DateTimeOffset.Now.Date,
                RequestId = request.RequestId,
                RequestTypeId = request.RequestTypeId,
            };
            IEnumerable<DateTime> disabledDates = lateInEarlyOutRequestService.GetDisabledDate(param);
            var viewModel = new LateInEarlyOutRequestViewModel
            {
                RequestId = request.RequestId,
                Employee = request.Requester.DisplayName,
                EmployeeId = request.Requester.EmpId,
                OnDate = request.OnDate,
                Minute = minutes,
                RequestType = request.RequestTypeId,
                Remark = request.Remark,
                ReasonId = request.ReasonId,
                ApproverId = request.Approver?.EmpId,
                VerifierId = request.Verifier?.EmpId,
                DisabledDates = disabledDates,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers,
                Files = attachFileService.GetAttachFile(id, UploadFolderPath)
            };

            ViewBag.IsTopManager = request.Requester.IsTopManager;
            return View(viewModel);
        }

        [HttpPost]
        public CustomJsonResult Edit(string requestModel, IList<HttpPostedFileBase> files, string filesRemove)
        {
            var model = JsonConverter.FromJson<LateInEarlyOutRequestViewModel>(requestModel);
            var criteria = new UpdateLateInEarlyOutRequestCriteria
            {
                RequestId = model.RequestId,
                EmpId = model.EmployeeId,
                OnDate = model.OnDate,
                RequestType = model.RequestType,
                Remark = model.Remark,
                ReasonId = model.ReasonId,
                LateInMins = model.RequestType == (short)RequestType.LateIn ? model.Minute : 0,
                EarlyOutMins = model.RequestType == (short)RequestType.EarlyOut ? model.Minute : 0,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                ModifiedBy = EmployeeSession.EmpId
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                lateInEarlyOutRequestService.SendMailEditRequest(model.RequestId);
                attachFileService.UploadFile(files, criteria.RequestId, UploadFolderPath);
                if (!string.IsNullOrWhiteSpace(filesRemove))
                {
                    attachFileService.DeleteFile(JsonConverter.FromJsonArray<string>(filesRemove), criteria.RequestId, UploadFolderPath);
                }
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public CustomJsonResult Delete(int id)
        {
            lateInEarlyOutRequestService.SendMailDeleteRequest(id);
            CommandResult commandResult = lateInEarlyOutRequestService.Delete(id);

            if (commandResult.IsSuccessful)
            {
                attachFileService.DeleteFolderAttachFile(id, UploadFolderPath);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ViewHistory(long id)
        {
            var requests = lateInEarlyOutRequestService.GetHistoryById(id);
            return PartialView("_ViewHistory", requests);
        }

        public CustomJsonResult GetActualLateInEarlyOut(int employeeId, DateTime workDate)
        {
            var criteria = new GetActualLateInEarlyOutCriteria
            {
                EmployeeId = employeeId,
                WorkDate = workDate
            };

            EmployeeLateInEarlyOutModel lateInEarlyOut = lateInEarlyOutRequestService.GetActualLateInEarlyOut(criteria);
            lateInEarlyOut = lateInEarlyOut ?? new EmployeeLateInEarlyOutModel();

            return new CustomJsonResult { Data = new { lateInEarlyOut.LateIn, lateInEarlyOut.EarlyOut }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public CustomJsonResult GetDisabledDate(GetDisabledDateParam param)
        {
            IEnumerable<DateTime> disabledDates = lateInEarlyOutRequestService.GetDisabledDate(param);
            return new CustomJsonResult { Data = disabledDates, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}