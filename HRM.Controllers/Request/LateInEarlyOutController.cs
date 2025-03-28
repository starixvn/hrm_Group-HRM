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
using HRM.Criteria.Request.LateInEarlyOut;
using HRM.Models.Request;
using HRM.Services.Interfaces.Request;
using HRM.ViewModels.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.Interfaces;

public class LateInEarlyOutController(
    IUserControlSession session,
    ILateInEarlyOutRequestService lateInEarlyOutRequestService,
    IRequestService requestService,
    IBaseService baseService,
    IAttachFileService attachFileService) : BaseController(session)
{
    private const int DefaultMinutes = 60;

    [HttpGet]
    public async Task<IActionResult> EmployeeView(int? year)
    {
        var tempYear = year ?? DateTimeOffset.Now.DateTime.ToClientTimeZone().Year;
        ViewBag.Year = tempYear;
        var fromDate = new DateTimeOffset(tempYear, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
        var criteria = new GetLateInEarlyOutRequestByDateCriteria
        {
            EmpId = EmployeeSession.EmpId,
            FromDate = fromDate,
            ToDate = fromDate.AddYears(1).AddDays(-1),
            CompanyId = CurrentCompanyId
        };
        var requests = await lateInEarlyOutRequestService.GetByDateAsync(criteria);
        foreach (var request in requests)
        {
            request.AttachFiles = await attachFileService.GetInfoAttachFileAsync(request.RequestId, $"{CurrentCompanyId}/requests");
            request.CreatedDate = request.CreatedDate.ToClientTimeZone();
        }

        return View(requests);
    }

    public async Task<IActionResult> Create()
    {
        var supervisor = await requestService.GetVerifierAndApproverAsync(EmployeeSession.EmpId, RequestType.LateIn);
        var viewModel = new LateInEarlyOutRequestViewModel
        {
            Employee = EmployeeSession.DisplayName,
            EmployeeId = EmployeeSession.EmpId,
            OnDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date,
            Minute = DefaultMinutes,
            ReasonId = (byte)LateInEarlyOutReason.Other,
            RequestType = (byte)RequestType.LateIn,
            Verifiers = supervisor.Verifiers,
            Approvers = supervisor.Approvers
        };
        ViewBag.IsTopManager = EmployeeSession.IsTopManager;
        ViewBag.HasOnBehalfRequestPermission = session.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
            || session.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string requestModel, IList<IFormFile> files)
    {
        var model = JsonConvert.DeserializeObject<LateInEarlyOutRequestViewModel>(requestModel);
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
            CreatedBy = EmployeeSession.EmpId,
            CompanyId = CurrentCompanyId
        };
        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        if (commandResult.IsSuccessful)
        {
            await lateInEarlyOutRequestService.SendMailAndSendLetstalkCreateRequestAsync(criteria.RequestId);
            await attachFileService.UploadFileAsync(files, criteria.RequestId, $"{CurrentCompanyId}/requests");
        }

        return Json(commandResult);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var request = await lateInEarlyOutRequestService.GetByIdAsync(id);
        if (request == null
            || !Utilities.CheckUserCanEditRequest(EmployeeSession.EmpId, request.Requester.EmpId)
            || request.Status.Id != (byte)CurrentRequestStatus.Pending)
        {
            return RedirectToAction("AccessDenied", "Error");
        }

        var supervisor = await requestService.GetVerifierAndApproverAsync(request.Requester.EmpId, RequestType.LateIn);
        var minutes = request.RequestTypeId == (byte)RequestType.LateIn ? request.LateInMins : request.EarlyOutMins;
        var param = new GetDisabledDateParam
        {
            EmployeeId = request.Requester.EmpId,
            FromDate = request.OnDate.AddMonths(-1),
            ToDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date,
            RequestId = request.RequestId,
            RequestTypeId = request.RequestTypeId
        };
        var disabledDates = await lateInEarlyOutRequestService.GetDisabledDateAsync(param);
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
            Files = (await attachFileService.GetInfoAttachFileAsync(id, $"{CurrentCompanyId}/requests")).Select(x => x.FileName).ToList()
        };

        ViewBag.IsTopManager = request.Requester.IsTopManager;
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string requestModel, IList<IFormFile> files, string filesRemove)
    {
        var model = JsonConvert.DeserializeObject<LateInEarlyOutRequestViewModel>(requestModel);
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
            ModifiedBy = EmployeeSession.EmpId,
            CompanyId = CurrentCompanyId
        };

        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        if (commandResult.IsSuccessful)
        {
            await lateInEarlyOutRequestService.SendMailEditRequestAsync(model.RequestId);
            await attachFileService.UploadFileAsync(files, criteria.RequestId, $"{CurrentCompanyId}/requests");
            if (!string.IsNullOrWhiteSpace(filesRemove))
            {
                await attachFileService.DeleteFileAsync(JsonConvert.DeserializeObject<List<string>>(filesRemove), criteria.RequestId, $"{CurrentCompanyId}/requests");
            }
        }

        return Json(commandResult);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await lateInEarlyOutRequestService.SendMailDeleteRequestAsync(id);
        var commandResult = await lateInEarlyOutRequestService.DeleteAsync(id);

        if (commandResult.IsSuccessful)
        {
            await attachFileService.DeleteFolderAttachFileAsync(id, $"{CurrentCompanyId}/requests");
        }

        return Json(commandResult);
    }

    public async Task<IActionResult> ViewHistory(long id)
    {
        var requests = await lateInEarlyOutRequestService.GetHistoryByIdAsync(id);
        return PartialView("_ViewHistory", requests);
    }

    public async Task<IActionResult> GetActualLateInEarlyOut(int employeeId, DateTime workDate)
    {
        var criteria = new GetActualLateInEarlyOutCriteria
        {
            EmployeeId = employeeId,
            WorkDate = workDate
        };

        var lateInEarlyOut = await lateInEarlyOutRequestService.GetActualLateInEarlyOutAsync(criteria);
        lateInEarlyOut ??= new EmployeeLateInEarlyOutModel();

        return Json(new { lateInEarlyOut.LateIn, lateInEarlyOut.EarlyOut });
    }

    public async Task<IActionResult> GetDisabledDate(GetDisabledDateParam param)
    {
        var disabledDates = await lateInEarlyOutRequestService.GetDisabledDateAsync(param);
        return Json(disabledDates);
    }
}