using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Services;
using HRM.Criteria.Request.KeepInView;
using HRM.Models.Request;
using HRM.Services;
using HRM.Services.Interfaces;
using HRM.Services.Interfaces.Request;
using HRM.ViewModels.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HRM.Controllers.Request;

public class KeepInViewController(
    IUserControlSession userControlSession,
    IKeepInViewRequestService keepInViewRequestService,
    IRequestService requestService,
    IBaseService baseService,
    IAttachFileService attachFileService,
    IShiftService shiftService) : BaseController(userControlSession)
{
    private const string UploadFolderPath = "/file/uploads/request/";

    [HttpGet]
    public async Task<IActionResult> EmployeeView(int? year)
    {
        int tempYear = year ?? DateTimeOffset.Now.Year;
        ViewBag.Year = tempYear;
        var fromDate = new DateTimeOffset(tempYear, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetKeepInViewRequestByDateCriteria
        {
            EmpId = EmployeeSession.EmpId,
            FromDate = fromDate,
            ToDate = fromDate.AddYears(1).AddDays(-1),
            CompanyId = companyId
        };
        var requests = await keepInViewRequestService.GetByDateAsync(criteria);
        foreach (var request in requests)
        {
            request.AttachFiles = attachFileService.GetInfoAttachFile(request.RequestId, UploadFolderPath);
            request.CreatedDate = request.CreatedDate.ToClientTimeZone();
            request.FromTime = request.FromTime.ToClientTimeZone();
            request.ToTime = request.ToTime.ToClientTimeZone();
        }

        return View(requests);
    }

    public async Task<IActionResult> Create()
    {
        var today = DateTimeOffset.Now.Date;
        var supervisor = await requestService.GetVerifierAndApproverAsync(EmployeeSession.EmpId, RequestType.KeepInView);
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
        ViewBag.Shifts = await shiftService.GetAllAsync();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string requestModel, IList<IFormFile> files)
    {
        var cultureInfo = new CultureInfo("en-US");
        var model = JsonConvert.DeserializeObject<KeepInViewRequestViewModel>(requestModel);
        var fromTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime, cultureInfo);
        var toTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime, cultureInfo);
        var fromTimeCrossDay = (fromTime < toTime && model.CrossDay)
            ? DateTime.Parse(model.Date.AddDays(1).ToDateMonthYearFormatedDate() + " " + model.FromTime, cultureInfo)
            : DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime, cultureInfo);
        var toTimeCrossDay = model.CrossDay
            ? DateTime.Parse(model.Date.AddDays(1).ToDateMonthYearFormatedDate() + " " + model.ToTime, cultureInfo)
            : DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime, cultureInfo);

        var criteria = new CreateKeepInViewRequestCriteria
        {
            EmpId = model.EmployeeId,
            OnDate = model.Date,
            KeepInViewType = model.KeepInViewType,
            IsHalfDay = model.HalfDay,
            Remark = model.Remark,
            FromTime = fromTimeCrossDay.ToServerTimeZone(),
            ToTime = toTimeCrossDay.ToServerTimeZone(),
            VerifierId = model.VerifierId,
            ApproverId = model.ApproverId,
            CreatedBy = EmployeeSession.EmpId,
            ShiftId = model.ShiftId
        };

        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        if (commandResult.IsSuccessful)
        {
            await keepInViewRequestService.SendMailAndSendLetstalkCreateRequestAsync(criteria.RequestId);
            attachFileService.UploadFile(files, criteria.RequestId, UploadFolderPath);
        }

        return Json(commandResult);
    }

    public async Task<IActionResult> Edit(long id)
    {
        var request = await keepInViewRequestService.GetByIdAsync(id);
        if (request == null
            || !Utilities.CheckUserCanEditRequest(EmployeeSession.EmpId, request.Requester.EmpId)
            || request.Status.Id != (byte)CurrentRequestStatus.Pending)
        {
            return RedirectToAction("AccessDenied", "Error");
        }

        var supervisor = await requestService.GetVerifierAndApproverAsync(request.Requester.EmpId, RequestType.KeepInView);
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
            FromTime = request.FromTime.ToClientTimeZone().ToString("HH:mm"),
            ToTime = request.ToTime.ToClientTimeZone().ToString("HH:mm"),
            Verifiers = supervisor.Verifiers,
            Approvers = supervisor.Approvers,
            MinDate = request.OnDate.AddMonths(-1),
            MaxDate = DateTimeOffset.Now.Date.FirstDayOfMonth().AddMonths(2).AddDays(-1),
            Files = attachFileService.GetAttachFile(id, UploadFolderPath),
            ShiftId = request.ShiftId
        };

        ViewBag.IsTopManager = request.Requester.IsTopManager;
        ViewBag.Shifts = await shiftService.GetAllAsync();

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string requestModel, IList<IFormFile> files, string filesRemove)
    {
        var cultureInfo = new CultureInfo("en-US");
        var model = JsonConvert.DeserializeObject<KeepInViewRequestViewModel>(requestModel);
        var fromTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime, cultureInfo);
        var toTimeCrossDay = model.CrossDay
            ? DateTime.Parse(model.Date.AddDays(1).ToDateMonthYearFormatedDate() + " " + model.ToTime, cultureInfo)
            : DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime, cultureInfo);
        var criteria = new EditKeepInViewRequestCriteria
        {
            RequestId = model.RequestId,
            EmpId = model.EmployeeId,
            OnDate = model.Date,
            KeepInViewType = model.KeepInViewType,
            Remark = model.Remark,
            IsHalfDay = model.HalfDay,
            FromTime = fromTime.ToServerTimeZone(),
            ToTime = toTimeCrossDay.ToServerTimeZone(),
            VerifierId = model.VerifierId,
            ApproverId = model.ApproverId,
            ModifiedBy = EmployeeSession.EmpId,
            ShiftId = model.ShiftId
        };

        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        if (commandResult.IsSuccessful)
        {
            await keepInViewRequestService.SendMailEditRequestAsync(model.RequestId);
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
        await keepInViewRequestService.SendMailDeleteRequestAsync(id);
        var commandResult = await keepInViewRequestService.DeleteAsync(id);

        if (commandResult.IsSuccessful)
        {
            attachFileService.DeleteFolderAttachFile(id, UploadFolderPath);
        }

        return Json(commandResult);
    }

    public async Task<IActionResult> ViewHistory(long id)
    {
        var requests = await keepInViewRequestService.GetHistoryByIdAsync(id);
        Parallel.ForEach(requests, (item) =>
        {
            item.FromTime = item.FromTime.ToClientTimeZone();
            item.ToTime = item.ToTime.ToClientTimeZone();
        });
        return PartialView("_ViewHistory", requests);
    }

    public async Task<IActionResult> GetDisabledDate(GetDisabledDateParam param)
    {
        var disabledDates = await keepInViewRequestService.GetDisabledDateAsync(param);
        return Json(disabledDates);
    }
}