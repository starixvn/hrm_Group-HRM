namespace HRM.Controllers.Request;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Enums;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Services;
using HRM.Criteria.Request.CheckInOut;
using HRM.Models.Request;
using HRM.Services.Interfaces.Request;
using HRM.ViewModels.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Shifts;
using Newtonsoft.Json;
using Services.Interfaces;

public class CheckInOutController(
    IUserControlSession session,
    ICheckInOutRequestService checkInOutRequestService,
    IRequestService requestService,
    IBaseService baseService,
    IShiftService shiftService,
    IAttachFileService attachFileService,
    IRequestMailService requestMailService) : BaseController(session)
{
    [HttpGet]
    public async Task<IActionResult> EmployeeView(int? year)
    {
        var tempYear = year ?? DateTimeOffset.Now.DateTime.ToClientTimeZone().Year;
        ViewBag.Year = tempYear;
        var fromDate = new DateTimeOffset(tempYear, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
        var criteria = new GetCheckInOutRequestByDateCriteria
        {
            EmpId = session.UserId,
            FromDate = fromDate,
            ToDate = fromDate.AddYears(1).AddDays(-1),
            CompanyId = CurrentCompanyId
        };

        var requests = await checkInOutRequestService.GetByDateAsync(criteria);
        foreach (var request in requests)
        {
            request.AttachFiles = await attachFileService.GetInfoAttachFileAsync(request.RequestId, $"{CurrentCompanyId}/requests");
            request.CreatedDate = request.CreatedDate.ToClientTimeZone();
            request.RequestTime = request.RequestTime.ToClientTimeZone();
        }

        return View(requests);
    }

    public async Task<IActionResult> Create()
    {
        var today = DateTimeOffset.Now.DateTime.ToClientTimeZone();
        var supervisor = await requestService.GetVerifierAndApproverAsync(session.UserId, RequestType.CheckIn);
        var viewModel = new CheckInOutRequestViewModel
        {
            Employee = EmployeeSession.DisplayName,
            EmployeeId = session.UserId,
            ForDate = today.Date,
            AttendantDate = today.Date,
            Hour = today.Hour,
            Minute = today.Minute,
            RequestTypeId = (byte)RequestType.CheckIn,
            Verifiers = supervisor.Verifiers,
            Approvers = supervisor.Approvers
        };

        var shift = await shiftService.GetByDateAsync(session.UserId, today.Date);
        CheckShiftFromToFormat(viewModel, shift);
        viewModel.Shift = shift == null ? string.Empty : shift.Name;
        ViewBag.Shift = shift;
        ViewBag.IsTopManager = EmployeeSession.IsTopManager;
        ViewBag.HasOnBehalfRequestPermission = session.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
            || session.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string requestModel, IList<IFormFile> files)
    {
        var model = JsonConvert.DeserializeObject<CheckInOutRequestViewModel>(requestModel);
        var requestTime = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);
        var criteria = new CreateCheckInOutRequestCriteria
        {
            EmpId = model.EmployeeId,
            OnDate = model.AttendantDate,
            RequestTime = requestTime.ToServerTimeZone(),
            RequestType = model.RequestTypeId,
            ReasonId = model.ReasonId,
            RefNo = model.RefNO,
            Remark = model.Remark,
            CreatedBy = session.UserId,
            VerifierId = model.VerifierId,
            ApproverId = model.ApproverId
        };
        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        if (commandResult.IsSuccessful)
        {
            await attachFileService.UploadFileAsync(files, criteria.RequestId, $"{CurrentCompanyId}/requests");
            if (criteria.ApproverId == null) return Json(commandResult);

            var mailConfig = await requestMailService.GetEmployeeMailConfig(criteria.ApproverId.Value);
            await checkInOutRequestService.SendMailAndSendLetstalkCreateRequestAsync(criteria.RequestId, mailConfig);
        }

        return Json(commandResult);
    }

    public async Task<IActionResult> Edit(long id)
    {
        var request = await checkInOutRequestService.GetByIdAsync(id);
        if (request == null
            || !Utilities.CheckUserCanEditRequest(session.UserId, request.Requester.EmpId)
            || request.Status.Id != (byte)CurrentRequestStatus.Pending)
        {
            return RedirectToAction("AccessDenied", "Error");
        }

        var supervisor = await requestService.GetVerifierAndApproverAsync(request.Requester.EmpId, RequestType.CheckIn);
        var param = new GetDisabledDateParam
        {
            EmployeeId = request.Requester.EmpId,
            FromDate = request.OnDate.AddMonths(-1),
            ToDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date,
            RequestId = request.RequestId,
            RequestTypeId = request.RequestTypeId
        };
        var disabledDates = await checkInOutRequestService.GetDisabledDateAsync(param);
        var shift = await shiftService.GetByDateAsync(session.UserId, request.OnDate);
        request.RequestTime = request.RequestTime.ToClientTimeZone();
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
            Files = (await attachFileService.GetInfoAttachFileAsync(id, $"{CurrentCompanyId}/requests")).Select(x => x.FileName).ToList()
        };

        ViewBag.IsTopManager = request.Requester.IsTopManager;
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string requestModel, IList<IFormFile> files, string filesRemove)
    {
        var model = JsonConvert.DeserializeObject<CheckInOutRequestViewModel>(requestModel);
        var requestTime = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);
        var criteria = new EditCheckInOutRequestCriteria
        {
            RequestId = model.RequestId,
            EmpId = model.EmployeeId,
            OnDate = model.AttendantDate,
            RequestTime = requestTime.ToServerTimeZone(),
            RequestType = model.RequestTypeId,
            ReasonId = model.ReasonId,
            RefNo = model.RefNO,
            Remark = model.Remark,
            ModifiedBy = session.UserId,
            VerifierId = model.VerifierId,
            ApproverId = model.ApproverId
        };

        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        if (commandResult.IsSuccessful)
        {
            await attachFileService.UploadFileAsync(files, criteria.RequestId, $"{CurrentCompanyId}/requests");
            if (!string.IsNullOrWhiteSpace(filesRemove))
            {
                await attachFileService.DeleteFileAsync(JsonConvert.DeserializeObject<List<string>>(filesRemove), criteria.RequestId, $"{CurrentCompanyId}/requests");
            }
            if (criteria.ApproverId == null) return Json(commandResult);

            var mailConfig = await requestMailService.GetEmployeeMailConfig(criteria.ApproverId.Value);
            await checkInOutRequestService.SendMailEditRequestAsync(model.RequestId, mailConfig);
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
            await attachFileService.DeleteFolderAttachFileAsync(id, $"{CurrentCompanyId}/requests");
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
            var checkContainColon = shift.BeginCheckInTime.Contains(':');
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