using HRM.Services.Interfaces;

namespace HRM.Controllers.Request
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using HRM.Controllers;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.Request.CheckInOut;
    using HRM.Models;
    using HRM.Models.Request;
    using HRM.Services;
    using HRM.Services.Interfaces.Request;
    using HRM.ViewModels.Request;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class CheckInOutController(
        IUserControlSession userControlSession,
        ICheckInOutRequestService checkInOutRequestService,
        IRequestService requestService,
        IBaseService baseService,
        IShiftService shiftService,
        IAttachFileService attachFileService) : BaseController(userControlSession)
    {
        private const string UploadFolderPath = "/file/uploads/request/";

        [HttpGet]
        public async Task<IActionResult> EmployeeView(int? year)
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

            var requests = await checkInOutRequestService.GetByDateAsync(criteria);
            foreach (var request in requests)
            {
                request.AttachFiles = attachFileService.GetInfoAttachFile(request.RequestId, UploadFolderPath);
            }

            return View(requests);
        }

        public async Task<IActionResult> Create()
        {
            var today = DateTimeOffset.Now;
            var supervisor = await requestService.GetVerifierAndApproverAsync(EmployeeSession.EmpId, RequestType.CheckIn);
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

            var shift = await shiftService.GetByDateAsync(EmployeeSession.EmpId, today.Date);
            CheckShiftFromToFormat(viewModel, shift);
            viewModel.Shift = shift == null ? string.Empty : shift.Name;
            ViewBag.Shift = shift;
            ViewBag.IsTopManager = EmployeeSession.IsTopManager;
            ViewBag.HasOnBehalfRequestPermission = userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                || userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string requestModel, IList<IFormFile> files)
        {
            var model = JsonConvert.DeserializeObject<CheckInOutRequestViewModel>(requestModel);
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
            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            if (commandResult.IsSuccessful)
            {
                await checkInOutRequestService.SendMailAndSendLetstalkCreateRequestAsync(criteria.RequestId);
                attachFileService.UploadFile(files, criteria.RequestId, UploadFolderPath);
            }

            return Json(commandResult);
        }

        public async Task<IActionResult> Edit(long id)
        {
            var request = await checkInOutRequestService.GetByIdAsync(id);
            if (request == null
                || !Utilities.CheckUserCanEditRequest(EmployeeSession.EmpId, request.Requester.EmpId)
                || request.Status.Id != (byte)CurrentRequestStatus.Pending)
            {
                return RedirectToAction("AccessDenied", "Error");
            }
            else
            {
                var supervisor = await requestService.GetVerifierAndApproverAsync(request.Requester.EmpId, RequestType.CheckIn);
                var param = new GetDisabledDateParam
                {
                    EmployeeId = request.Requester.EmpId,
                    FromDate = request.OnDate.AddMonths(-1),
                    ToDate = DateTimeOffset.Now.Date,
                    RequestId = request.RequestId,
                    RequestTypeId = request.RequestTypeId
                };
                var disabledDates = await checkInOutRequestService.GetDisabledDateAsync(param);
                var shift = await shiftService.GetByDateAsync(EmployeeSession.EmpId, request.OnDate);
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
        public async Task<IActionResult> Edit(string requestModel, IList<IFormFile> files, string filesRemove)
        {
            var model = JsonConvert.DeserializeObject<CheckInOutRequestViewModel>(requestModel);
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

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
            if (commandResult.IsSuccessful)
            {
                await checkInOutRequestService.SendMailEditRequestAsync(model.RequestId);
                attachFileService.UploadFile(files, criteria.RequestId, UploadFolderPath);
                if (!string.IsNullOrWhiteSpace(filesRemove))
                {
                    attachFileService.DeleteFile(JsonConvert.DeserializeObject<List<string>>(filesRemove), criteria.RequestId, UploadFolderPath);
                }
            }

            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await checkInOutRequestService.SendMailDeleteRequestAsync(id);
            var commandResult = await checkInOutRequestService.DeleteAsync(id);

            if (commandResult.IsSuccessful)
            {
                attachFileService.DeleteFolderAttachFile(id, UploadFolderPath);
            }

            return Json(commandResult);
        }

        public async Task<IActionResult> ViewHistory(long id)
        {
            var requests = await checkInOutRequestService.GetHistoryByIdAsync(id);
            return PartialView("_ViewHistory", requests);
        }

        public async Task<IActionResult> GetDisabledDate(GetDisabledDateParam param)
        {
            var disabledDates = await checkInOutRequestService.GetDisabledDateAsync(param);
            return Json(disabledDates);
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