using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Services;
using HRM.Criteria.Request.KeepInView;
using HRM.Models.Request;
using HRM.Services.Interfaces;
using HRM.Services.Interfaces.Request;
using HRM.ViewModels.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HRM.Controllers.Request;

public class KeepInViewController(
    IUserControlSession session,
    IKeepInViewRequestService keepInViewRequestService,
    IRequestService requestService,
    IBaseService baseService,
    IAttachFileService attachFileService,
    IShiftService shiftService,
    ILeaveRequestService leaveRequestService,
    IEmployeeService employeeService) : BaseController(session)
{
    [HttpGet]
    public async Task<IActionResult> EmployeeView(int? year)
    {
        var tempYear = year ?? DateTimeOffset.Now.DateTime.ToClientTimeZone().Year;
        ViewBag.Year = tempYear;
        var fromDate = new DateTimeOffset(tempYear, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
        var criteria = new GetKeepInViewRequestByDateCriteria
        {
            EmpId = EmployeeSession.EmpId,
            FromDate = fromDate,
            ToDate = fromDate.AddYears(1).AddDays(-1),
            CompanyId = CurrentCompanyId
        };
        var requests = await keepInViewRequestService.GetByDateAsync(criteria);
        foreach (var request in requests)
        {
            request.AttachFiles = await attachFileService.GetInfoAttachFileAsync(request.RequestId, $"{CurrentCompanyId}/requests");
            request.CreatedDate = request.CreatedDate.ToClientTimeZone();
            request.FromTime = request.FromTime.ToClientTimeZone();
            request.ToTime = request.ToTime.ToClientTimeZone();
        }
        ViewBag.EmployeeBalance = await leaveRequestService.GetLeaveBalance(EmployeeSession.EmpId, null, tempYear);
        return View(requests);
    }

    public async Task<IActionResult> Create()
    {
        var today = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
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
        ViewBag.AutoApproveRequest = EmployeeSession.AutoApproveRequest;
        ViewBag.HasOnBehalfRequestPermission = session.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
            || session.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string requestModel, IList<IFormFile> files)
    {
        var cultureInfo = new CultureInfo("en-US");
        var model = JsonConvert.DeserializeObject<KeepInViewRequestViewModel>(requestModel);
        var fromTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.FromTime, cultureInfo);
        var toTime = DateTime.Parse(model.Date.ToDateMonthYearFormatedDate() + " " + model.ToTime, cultureInfo);
        var fromTimeCrossDay = fromTime < toTime && model.CrossDay
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
            CompanyId = CurrentCompanyId
        };

        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        if (commandResult.IsSuccessful)
        {
            await attachFileService.UploadFileAsync(files, criteria.RequestId, $"{CurrentCompanyId}/requests");
            await keepInViewRequestService.SendMailAndSendLetstalkCreateRequestAsync(criteria.RequestId);
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
            Files = (await attachFileService.GetInfoAttachFileAsync(id, $"{CurrentCompanyId}/requests")).Select(x => x.FileName).ToList(),
        };

        viewModel.Verifiers = await employeeService.CheckAndAddMissingSupervisor(viewModel.Verifiers, viewModel.VerifierId);
        viewModel.Approvers = await employeeService.CheckAndAddMissingSupervisor(viewModel.Approvers, viewModel.ApproverId);

        ViewBag.AutoApproveRequest = request.Requester.AutoApproveRequest;
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
            CompanyId = CurrentCompanyId
        };

        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        if (commandResult.IsSuccessful)
        {
            await keepInViewRequestService.SendMailEditRequestAsync(model.RequestId);
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
        await keepInViewRequestService.SendMailDeleteRequestAsync(id);
        var commandResult = await keepInViewRequestService.DeleteAsync(id);

        if (commandResult.IsSuccessful)
        {
            await attachFileService.DeleteFolderAttachFileAsync(id, $"{CurrentCompanyId}/requests");
        }

        return Json(commandResult);
    }

    public async Task<IActionResult> ViewHistory(long id)
    {
        var requests = await keepInViewRequestService.GetHistoryByIdAsync(id);
        Parallel.ForEach(requests, item =>
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