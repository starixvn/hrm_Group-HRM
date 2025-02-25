using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.Request.KeepInView;
    using HRM.Models.Request;
    using HRM.Services;
    using HRM.ViewModels.Request;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class KeepInViewController : BaseController
    {
        private const string UploadFolderPath = "/file/uploads/request/";
        private readonly IUserControlSession userControlSession;
        private readonly IKeepInViewRequestService keepInViewRequestService;
        private readonly IRequestService requestService;
        private readonly IBaseService baseService;
        private readonly IAttachFileService attachFileService;
        private readonly IShiftService shiftService;

        public KeepInViewController(
            IUserControlSession userControlSession,
            IKeepInViewRequestService keepInViewRequestService,
            IRequestService requestService,
            IBaseService baseService,
            IAttachFileService attachFileService,
            IShiftService shiftService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.keepInViewRequestService = keepInViewRequestService;
            this.requestService = requestService;
            this.baseService = baseService;
            this.attachFileService = attachFileService;
            this.shiftService = shiftService;
        }

        [HttpGet]
        public ActionResult EmployeeView(int? year)
        {
            int tempYear = year ?? DateTimeOffset.Now.Year;
            ViewBag.Year = tempYear;
            var fromDate = new DateTimeOffset(tempYear, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
            var criteria = new GetKeepInViewRequestByDateCriteria
            {
                EmpId = EmployeeSession.EmpId,
                FromDate = fromDate,
                ToDate = fromDate.AddYears(1).AddDays(-1)
            };
            IList<KeepInViewRequest> requests = keepInViewRequestService.GetByDate(criteria);
            foreach (var request in requests)
            {
                request.AttachFiles = attachFileService.GetInfoAttachFile(request.RequestId, UploadFolderPath);
            }

            return View(requests);
        }

        public ActionResult Create()
        {
            var today = DateTimeOffset.Now.Date;
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(EmployeeSession.EmpId, RequestType.KeepInView);
            var model = new KeepInViewRequestViewModel
            {
                Employee = EmployeeSession.DisplayName,
                EmployeeId = EmployeeSession.EmpId,
                Date = today,
                KeepInViewType = (byte)KeepInViewTypes.KeepInViewByDay,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers,
                MinDate = today.FirstDayOfMonth().AddMonths(-1),
                MaxDate = today.FirstDayOfMonth().AddMonths(2).AddDays(-1)
            };
            ViewBag.IsTopManager = EmployeeSession.IsTopManager;
            ViewBag.HasOnBehalfRequestPermission = userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);
            ViewBag.Shifts = shiftService.GetAll();

            return View(model);
        }

        [HttpPost]
        public CustomJsonResult Create(string requestModel, IList<HttpPostedFileBase> files)
        {
            var cultureInfo = new CultureInfo("en-US");
            var model = JsonConverter.FromJson<KeepInViewRequestViewModel>(requestModel);
            var fromTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime, cultureInfo);
            var toTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime, cultureInfo);
            var criteria = new CreateKeepInViewRequestCriteria
            {
                EmpId = model.EmployeeId,
                OnDate = model.Date,
                KeepInViewType = model.KeepInViewType,
                IsHalfDay = model.HalfDay,
                Remark = model.Remark,
                FromTime = (fromTime < toTime && model.CrossDay)
                        ? DateTime.Parse(model.Date.AddDays(1).ToDateMonthYearFormatedDate() + " " + model.FromTime, cultureInfo)
                        : DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime, cultureInfo),
                ToTime = model.CrossDay
                        ? DateTime.Parse(model.Date.AddDays(1).ToDateMonthYearFormatedDate() + " " + model.ToTime, cultureInfo)
                        : DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime, cultureInfo),
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                CreatedBy = EmployeeSession.EmpId,
                ShiftId = model.ShiftId
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                keepInViewRequestService.SendMailAndSendLetstalkCreateRequest(criteria.RequestId);
                attachFileService.UploadFile(files, criteria.RequestId, UploadFolderPath);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Edit(long id)
        {
            KeepInViewRequest request = keepInViewRequestService.GetById(id);
            if (request == null
                || !Utilities.CheckUserCanEditRequest(EmployeeSession.EmpId, request.Requester.EmpId)
                || request.Status.Id != (byte)CurrentRequestStatus.Pending)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(request.Requester.EmpId, RequestType.KeepInView);
            var viewModel = new KeepInViewRequestViewModel
            {
                RequestId = id,
                RefNo = request.RefNO,
                HalfDay = request.IsHalfDay,
                Employee = request.Requester.DisplayName,
                EmployeeId = request.Requester.EmpId,
                Date = request.OnDate,
                KeepInViewType = request.KeepInViewTypeId,
                ApproverId = request.Approver?.EmpId,
                VerifierId = request.Verifier?.EmpId,
                Remark = request.Remark,
                CrossDay = (request.ToTime - request.OnDate).Days > 0,
                FromTime = request.FromTime.ToString("HH:mm"),
                ToTime = request.ToTime.ToString("HH:mm"),
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers,
                MinDate = request.OnDate.AddMonths(-1),
                MaxDate = DateTimeOffset.Now.Date.FirstDayOfMonth().AddMonths(2).AddDays(-1),
                Files = attachFileService.GetAttachFile(id, UploadFolderPath),
                ShiftId = request.ShiftId
            };

            ViewBag.IsTopManager = request.Requester.IsTopManager;
            ViewBag.Shifts = shiftService.GetAll();

            return View(viewModel);
        }

        [HttpPost]
        public CustomJsonResult Edit(string requestModel, IList<HttpPostedFileBase> files, string filesRemove)
        {
            var cultureInfo = new CultureInfo("en-US");
            var model = JsonConverter.FromJson<KeepInViewRequestViewModel>(requestModel);
            var criteria = new EditKeepInViewRequestCriteria
            {
                RequestId = model.RequestId,
                EmpId = model.EmployeeId,
                OnDate = model.Date,
                KeepInViewType = model.KeepInViewType,
                Remark = model.Remark,
                IsHalfDay = model.HalfDay,
                FromTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime, cultureInfo),
                ToTime = model.CrossDay
                        ? DateTime.Parse(model.Date.AddDays(1).ToDateMonthYearFormatedDate() + " " + model.ToTime, cultureInfo)
                        : DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime, cultureInfo),
                VerifierId = model.VerifierId,
                ApproverId = model.ApproverId,
                ModifiedBy = EmployeeSession.EmpId,
                ShiftId = model.ShiftId
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                keepInViewRequestService.SendMailEditRequest(model.RequestId);
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
            keepInViewRequestService.SendMailDeleteRequest(id);
            CommandResult commandResult = keepInViewRequestService.Delete(id);

            if (commandResult.IsSuccessful)
            {
                attachFileService.DeleteFolderAttachFile(id, UploadFolderPath);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ViewHistory(long id)
        {
            IList<KeepInViewRequest> requests = keepInViewRequestService.GetHistoryById(id);
            return PartialView("_ViewHistory", requests);
        }

        public CustomJsonResult GetDisabledDate(GetDisabledDateParam param)
        {
            IEnumerable<DateTime> disabledDates = keepInViewRequestService.GetDisabledDate(param);
            return new CustomJsonResult { Data = disabledDates, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}