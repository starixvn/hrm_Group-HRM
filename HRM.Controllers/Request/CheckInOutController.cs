using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.Request.CheckInOut;
    using HRM.Models;
    using HRM.Models.Request;
    using HRM.Services;
    using HRM.ViewModels.Request;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class CheckInOutController : BaseController
    {
        private const string UploadFolderPath = "/file/uploads/request/";
        private readonly IUserControlSession userControlSession;
        private readonly ICheckInOutRequestService checkInOutRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;
        private readonly IShiftService shiftService;
        private readonly IAttachFileService attachFileService;

        public CheckInOutController(
            IUserControlSession userControlSession,
            ICheckInOutRequestService checkInOutRequestService,
            IRequestService requestService,
            IBaseService baseService,
            IShiftService shiftService,
            IAttachFileService attachFileService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.checkInOutRequestService = checkInOutRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
            this.shiftService = shiftService;
            this.attachFileService = attachFileService;
        }

        [HttpGet]
        public ActionResult EmployeeView(int? year)
        {
            int tempYear = year ?? DateTimeOffset.Now.Year;
            ViewBag.Year = tempYear;
            var fromDate = new DateTimeOffset(tempYear, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
            var criteria = new GetCheckInOutRequestByDateCriteria
            {
                EmpId = EmployeeSession.EmpId,
                FromDate = fromDate,
                ToDate = fromDate.AddYears(1).AddDays(-1)
            };

            IList<CheckInOutRequest> requests = checkInOutRequestService.GetByDate(criteria);
            foreach (var request in requests)
            {
                request.AttachFiles = attachFileService.GetInfoAttachFile(request.RequestId, UploadFolderPath);
            }

            return View(requests);
        }

        public ActionResult Create()
        {
            var today = DateTimeOffset.Now;
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(EmployeeSession.EmpId, RequestType.CheckIn);
            var viewModel = new CheckInOutRequestViewModel
            {
                Employee = EmployeeSession.DisplayName,
                EmployeeId = EmployeeSession.EmpId,
                ForDate = today.Date,
                AttendantDate = today.Date,
                Hour = today.Hour,
                Minute = today.Minute,
                RequestTypeId = (byte)RequestType.CheckIn,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers
            };

            Shift shift = shiftService.GetByDate(EmployeeSession.EmpId, today.Date);
            CheckShiftFromToFormat(viewModel, shift);
            viewModel.Shift = shift == null ? string.Empty : shift.Name;
            ViewBag.Shift = shift;
            ViewBag.IsTopManager = EmployeeSession.IsTopManager;
            ViewBag.HasOnBehalfRequestPermission = userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

            return View(viewModel);
        }

        [HttpPost]
        public CustomJsonResult Create(string requestModel, IList<HttpPostedFileBase> files)
        {
            var model = JsonConverter.FromJson<CheckInOutRequestViewModel>(requestModel);
            var criteria = new CreateCheckInOutRequestCriteria
            {
                EmpId = model.EmployeeId,
                OnDate = model.AttendantDate,
                RequestTime = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0),
                RequestType = model.RequestTypeId,
                ReasonId = model.ReasonId,
                RefNo = model.RefNO,
                Remark = model.Remark,
                CreatedBy = EmployeeSession.EmpId,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            if (commandResult.IsSuccessful)
            {
                checkInOutRequestService.SendMailAndSendLetstalkCreateRequest(criteria.RequestId);
                attachFileService.UploadFile(files, criteria.RequestId, UploadFolderPath);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Edit(long id)
        {
            CheckInOutRequest request = checkInOutRequestService.GetById(id);
            if (request == null
                || !Utilities.CheckUserCanEditRequest(EmployeeSession.EmpId, request.Requester.EmpId)
                || request.Status.Id != (byte)CurrentRequestStatus.Pending)
            {
                return RedirectToAction("AccessDenied", "Error");
            }
            else
            {
                VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(request.Requester.EmpId, RequestType.CheckIn);
                var param = new GetDisabledDateParam
                {
                    EmployeeId = request.Requester.EmpId,
                    FromDate = request.OnDate.AddMonths(-1),
                    ToDate = DateTimeOffset.Now.Date,
                    RequestId = request.RequestId,
                    RequestTypeId = request.RequestTypeId
                };
                IEnumerable<DateTime> disabledDates = checkInOutRequestService.GetDisabledDate(param);
                Shift shift = shiftService.GetByDate(EmployeeSession.EmpId, request.OnDate);
                var viewModel = new CheckInOutRequestViewModel
                {
                    RequestId = request.RequestId,
                    RefNO = request.RefNO,
                    RequestTypeId = request.RequestTypeId,
                    Employee = request.Requester.DisplayName,
                    EmployeeId = request.Requester.EmpId,
                    AttendantDate = request.OnDate,
                    ForDate = request.RequestTime.Date,
                    Hour = request.RequestTime.Hour,
                    Minute = request.RequestTime.Minute,
                    ReasonId = request.ReasonId,
                    Shift = shift == null ? string.Empty : shift.Name,
                    ApproverId = request.Approver?.EmpId,
                    VerifierId = request.Verifier?.EmpId,
                    Remark = request.Remark,
                    Verifiers = supervisor.Verifiers,
                    Approvers = supervisor.Approvers,
                    DisabledDates = disabledDates,
                    Files = attachFileService.GetAttachFile(id, UploadFolderPath)
                };

                ViewBag.IsTopManager = request.Requester.IsTopManager;
                return View(viewModel);
            }
        }

        [HttpPost]
        public CustomJsonResult Edit(string requestModel, IList<HttpPostedFileBase> files, string filesRemove)
        {
            var model = JsonConverter.FromJson<CheckInOutRequestViewModel>(requestModel);
            var criteria = new EditCheckInOutRequestCriteria
            {
                RequestId = model.RequestId,
                EmpId = model.EmployeeId,
                OnDate = model.AttendantDate,
                RequestTime = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0),
                RequestType = model.RequestTypeId,
                ReasonId = model.ReasonId,
                RefNo = model.RefNO,
                Remark = model.Remark,
                ModifiedBy = EmployeeSession.EmpId,
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                checkInOutRequestService.SendMailEditRequest(model.RequestId);
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
            checkInOutRequestService.SendMailDeleteRequest(id);
            CommandResult commandResult = checkInOutRequestService.Delete(id);

            if (commandResult.IsSuccessful)
            {
                attachFileService.DeleteFolderAttachFile(id, UploadFolderPath);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ViewHistory(long id)
        {
            IList<CheckInOutRequest> requests = checkInOutRequestService.GetHistoryById(id);
            return PartialView("_ViewHistory", requests);
        }

        public CustomJsonResult GetDisabledDate(GetDisabledDateParam param)
        {
            IEnumerable<DateTime> disabledDates = checkInOutRequestService.GetDisabledDate(param);
            return new CustomJsonResult { Data = disabledDates, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private static void CheckShiftFromToFormat(CheckInOutRequestViewModel viewModel, Shift shift)
        {
            if (shift != null)
            {
                var checkContainColon = shift.BeginCheckInTime.Contains(":");
                if (checkContainColon)
                {
                    var time = shift.BeginCheckInTime.Split(':');
                    viewModel.Hour = int.Parse(time[0]);
                    viewModel.Minute = int.Parse(time[1]);
                }
                else
                {
                    viewModel.Hour = 0;
                    viewModel.Minute = 0;
                }
            }
        }
    }
}