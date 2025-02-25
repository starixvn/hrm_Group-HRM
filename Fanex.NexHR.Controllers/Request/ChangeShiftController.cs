namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Request.ChangeShift;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Request;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;

    public class ChangeShiftController : BaseController
    {
        private const string UploadFolderPath = "/file/uploads/request/";
        private readonly IUserControlSession userControlSession;
        private readonly IChangeShiftRequestService changeShiftService;
        private readonly IRequestService requestService;
        private readonly IShiftService shiftService;
        private readonly IAttachFileService attachFileService;

        public ChangeShiftController(
            IUserControlSession userControlSession,
            IChangeShiftRequestService changeShiftService,
            IRequestService requestService,
            IShiftService shiftService,
            IAttachFileService attachFileService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.changeShiftService = changeShiftService;
            this.requestService = requestService;
            this.shiftService = shiftService;
            this.attachFileService = attachFileService;
        }

        [HttpGet]
        public ActionResult EmployeeView(int? year)
        {
            int tempYear = year ?? DateTimeOffset.Now.Year;
            var fromDate = new DateTimeOffset(tempYear, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
            var toDate = fromDate.AddYears(1).AddDays(-1).Date.ToEndOfDay();
            IEnumerable<ChangeShiftRequest> requests = changeShiftService.GetByEmployee(EmployeeSession.EmpId, fromDate, toDate);
            foreach (var request in requests)
            {
                request.AttachFiles = attachFileService.GetInfoAttachFile(request.RequestId, UploadFolderPath);
            }
            ViewBag.Year = tempYear;

            return View(requests);
        }

        public ActionResult Create()
        {
            var today = DateTimeOffset.Now.Date;
            VerifierAndApproverForRequest supervisor = GetVerifierApprover(EmployeeSession.EmpId);
            var model = new ChangeShiftViewModel
            {
                Employee = EmployeeSession.DisplayName,
                EmployeeId = EmployeeSession.EmpId,
                FromDate = today,
                ToDate = today,
                TotalRequestInMonth = changeShiftService.GetNumberOfRequest(EmployeeSession.EmpId, today.FirstDayOfMonth(), today.LastDayOfMonth()),
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers
            };
            ViewBag.IsHost = true;
            ViewBag.IsTopManager = EmployeeSession.IsTopManager;
            ViewBag.HasOnBehalfRequestPermission = userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public CustomJsonResult Create(string model, string details, IList<HttpPostedFileBase> files)
        {
            var criteria = JsonConverter.FromJson<CreateChangeShift>(model);
            var requestDetail = JsonConverter.FromJsonArray<ChangeShiftDetail>(details);
            criteria.Details = requestDetail;

            CommandResult commandResult = changeShiftService.Create(EmployeeSession.EmpId, criteria);

            if (commandResult.IsSuccessful)
            {
                changeShiftService.SendMailAndSendLetstalkCreateRequest(commandResult.OutputId);
                attachFileService.UploadFile(files, commandResult.OutputId, UploadFolderPath);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Edit(int id)
        {
            ChangeShiftRequest request = changeShiftService.GetById(id);
            if (request == null
                || !Utilities.CheckUserCanEditRequest(EmployeeSession.EmpId, request.Requester.EmpId)
                || request.Status.Id != (byte)CurrentRequestStatus.Pending)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            VerifierAndApproverForRequest supervisor = GetVerifierApprover(request.Requester.EmpId);
            var model = new ChangeShiftViewModel
            {
                RequestId = request.RequestId,
                Employee = request.Requester.DisplayName,
                EmployeeId = request.Requester.EmpId,
                FromDate = request.FromDate.Date,
                ToDate = request.ToDate.Date,
                VerifierId = request.Verifier?.EmpId,
                ApproverId = request.Approver?.EmpId,
                Remark = request.Remark,
                Details = changeShiftService.GetDetailById(id),
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers,
                Files = attachFileService.GetAttachFile(id, UploadFolderPath)
            };

            if (request.TypeId == (byte)ChangeShiftType.MoveOffDay)
            {
                foreach (ChangeShiftDetail detail in model.Details)
                {
                    detail.DisableDates = changeShiftService.GetDisableDateForMoveOffDayRequest(model.EmployeeId, detail.OnDate, model.RequestId);
                }
            }

            ViewBag.IsHost = true;
            ViewBag.IsTopManager = request.Requester.IsTopManager;
            ViewBag.TypeId = request.TypeId;
            ViewBag.Shifts = shiftService.GetAll(isShowOff: true);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public CustomJsonResult Edit(string model, string details, IList<HttpPostedFileBase> files, string filesRemove)
        {
            var criteria = JsonConverter.FromJson<EditChangeShift>(model);
            var requestDetail = JsonConverter.FromJsonArray<ChangeShiftDetail>(details);
            criteria.Details = requestDetail;

            CommandResult commandResult = changeShiftService.Edit(EmployeeSession.EmpId, criteria);
            if (commandResult.IsSuccessful)
            {
                changeShiftService.SendMailEditRequest(criteria.RequestId);
                attachFileService.UploadFile(files, criteria.RequestId, UploadFolderPath);
                if (!string.IsNullOrWhiteSpace(filesRemove))
                {
                    attachFileService.DeleteFile(JsonConverter.FromJsonArray<string>(filesRemove), criteria.RequestId, UploadFolderPath);
                }
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public CustomJsonResult Delete(long id)
        {
            changeShiftService.SendMailDeleteRequest(id);
            CommandResult commandResult = changeShiftService.Delete(EmployeeSession.EmpId, id);

            if (commandResult.IsSuccessful)
            {
                attachFileService.DeleteFolderAttachFile(id, UploadFolderPath);
            }

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Details(int id)
        {
            ChangeShiftRequest request = changeShiftService.GetById(id);
            if (request == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            VerifierAndApproverForRequest supervisor = GetVerifierApprover(request.Requester.EmpId);
            var model = new ChangeShiftViewModel
            {
                Employee = request.Requester.DisplayName,
                EmployeeId = request.Requester.EmpId,
                FromDate = request.FromDate.Date,
                ToDate = request.ToDate.Date,
                VerifierId = request.Verifier?.EmpId,
                ApproverId = request.Approver?.EmpId,
                Remark = request.Remark,
                Details = changeShiftService.GetDetailById(id),
                Status = request.Status,
                Verifiers = supervisor.Verifiers,
                Approvers = supervisor.Approvers,
                Files = attachFileService.GetAttachFile(id, UploadFolderPath)
            };

            ViewBag.IsHost = false;
            ViewBag.TypeId = request.TypeId;
            ViewBag.Shifts = shiftService.GetAll(isShowOff: true);
            return View(model);
        }

        public ActionResult GetChangeShiftDetail(GetWorkingCalendarForRequestParam param)
        {
            IEnumerable<DailySchedule> dailySchedules = changeShiftService.GetWorkingCalendarForRequest(param);
            IList<ChangeShiftDetail> details = dailySchedules.Select(x => new ChangeShiftDetail
            {
                OnDate = x.WorkDate,
                Remark = string.Empty,
                FromShiftId = GetFromShiftId(x),
                FromShift = GetFromShiftName(x)
            }).ToList();

            if (param.TypeId == (byte)ChangeShiftType.MoveOffDay)
            {
                foreach (ChangeShiftDetail detail in details)
                {
                    detail.DisableDates = changeShiftService.GetDisableDateForMoveOffDayRequest(param.EmployeeId, detail.OnDate, param.RequestId);
                }
            }

            ViewBag.IsHost = true;
            ViewBag.Shifts = shiftService.GetAll(isShowOff: true);
            ViewBag.TypeId = param.TypeId;

            return PartialView("_ChangeShiftDetail", details);
        }

        public ActionResult ViewHistory(long id)
        {
            IEnumerable<ChangeShiftRequest> requests = changeShiftService.GetHistoryById(id);
            return PartialView("_ViewHistory", requests);
        }

        private static int GetFromShiftId(DailySchedule dailySchedule)
        {
            if (dailySchedule.IsOffDay)
            {
                return Constants.OffDayShiftId;
            }

            return dailySchedule.Shift != null ? dailySchedule.Shift.Id : 0;
        }

        private static string GetFromShiftName(DailySchedule dailySchedule)
        {
            if (dailySchedule.IsOffDay)
            {
                return Labels.Off;
            }

            return dailySchedule.Shift != null ? dailySchedule.Shift.Name : Labels.NA;
        }

        private VerifierAndApproverForRequest GetVerifierApprover(int employeeId, RequestEntity request = null)
        {
            VerifierAndApproverForRequest supervisor = requestService.GetVerifierAndApprover(employeeId, RequestType.ChangeShift);

            if (request != null)
            {
                if (request.VerifiedBy != null && !supervisor.Verifiers.Any(t => t.OwnerId == request.VerifiedBy.Id))
                {
                    var verifier = new SupervisorNameModel
                    {
                        OwnerId = request.VerifiedBy.Id,
                        NickName = request.VerifiedBy.NickName,
                        FullName = request.VerifiedBy.FullName
                    };
                    supervisor.Verifiers.Add(verifier);
                }

                if (request.ApprovedBy != null && !supervisor.Approvers.Any(t => t.OwnerId == request.ApprovedBy.Id))
                {
                    var approver = new SupervisorNameModel
                    {
                        OwnerId = request.ApprovedBy.Id,
                        NickName = request.ApprovedBy.NickName,
                        FullName = request.ApprovedBy.FullName
                    };
                    supervisor.Approvers.Add(approver);
                }
            }

            return supervisor;
        }
    }
}