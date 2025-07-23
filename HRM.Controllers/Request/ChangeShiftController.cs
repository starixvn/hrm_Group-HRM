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
using HRM.Criteria.Request.ChangeShift;
using HRM.Models.Request;
using HRM.Models.Request.ChangeShift;
using HRM.Services.Interfaces.Request;
using HRM.ViewModels.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.ScheduleArrange;
using Models.Supervisor;
using Newtonsoft.Json;
using Resources;
using Services.Interfaces;

public class ChangeShiftController(
    IUserControlSession session,
    IChangeShiftRequestService changeShiftService,
    IRequestService requestService,
    IShiftService shiftService,
    IAttachFileService attachFileService,
    IEmployeeService employeeService) : BaseController(session)
{
    [HttpGet]
    public async Task<IActionResult> EmployeeView(int? year)
    {
        var tempYear = year ?? DateTimeOffset.Now.Year;
        var fromDate = new DateTimeOffset(tempYear, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
        var toDate = fromDate.AddYears(1).AddDays(-1).Date.ToEndOfDay();
        var criteria = new GetChangeShiftRequestByEmployeeCriteria
        {
            EmployeeId = session.UserId,
            FromDate = fromDate,
            ToDate = toDate,
            CompanyId = CurrentCompanyId
        };
        var requests = await changeShiftService.GetByEmployeeAsync(criteria);
        foreach (var request in requests)
        {
            request.AttachFiles = await attachFileService.GetInfoAttachFileAsync(request.RequestId, $"{CurrentCompanyId}/requests");
            request.CreatedDate = request.CreatedDate.ToClientTimeZone();
        }
        ViewBag.Year = tempYear;

        return View(requests);
    }

    public async Task<IActionResult> Create()
    {
        var today = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        var supervisor = await GetVerifierApprover(session.UserId);
        var model = new ChangeShiftViewModel
        {
            Employee = EmployeeSession.DisplayName,
            EmployeeId = session.UserId,
            FromDate = today,
            ToDate = today,
            Verifiers = supervisor.Verifiers,
            Approvers = supervisor.Approvers
        };
        ViewBag.IsHost = true;
        ViewBag.AutoApproveRequest = EmployeeSession.AutoApproveRequest;
        ViewBag.HasOnBehalfRequestPermission = session.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
            || session.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string model, string details, IList<IFormFile> files)
    {
        var criteria = JsonConvert.DeserializeObject<CreateChangeShift>(model);
        var requestDetail = JsonConvert.DeserializeObject<List<ChangeShiftDetail>>(details);
        criteria.Details = requestDetail;

        var commandResult = await changeShiftService.CreateAsync(session.UserId, criteria);

        if (commandResult.IsSuccessful)
        {
            await attachFileService.UploadFileAsync(files, commandResult.OutputId, $"{CurrentCompanyId}/requests");
            await changeShiftService.SendMailAndSendLetstalkCreateRequestAsync(commandResult.OutputId);
        }

        return Json(commandResult);
    }

    public async Task<ActionResult> Edit(int id)
    {
        var request = await changeShiftService.GetByIdAsync(id);
        if (request == null
            || !Utilities.CheckUserCanEditRequest(session.UserId, request.Requester.EmpId)
            || request.Status.Id != (byte)CurrentRequestStatus.Pending)
        {
            return RedirectToAction("AccessDenied", "Error");
        }

        var supervisor = await GetVerifierApprover(request.Requester.EmpId);
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
            Details = await changeShiftService.GetDetailByIdAsync(id),
            Verifiers = supervisor.Verifiers,
            Approvers = supervisor.Approvers,
            Files = (await attachFileService.GetInfoAttachFileAsync(id, $"{CurrentCompanyId}/requests")).Select(x => x.FileName).ToList()
        };

        if (model.Verifiers.Any() && model.VerifierId.HasValue)
        {
            model.Verifiers = await employeeService.CheckAndAddMissingSupervisor(model.Verifiers, model.VerifierId.Value);
        }

        if (model.Approvers.Any() && model.ApproverId.HasValue)
        {
            model.Approvers = await employeeService.CheckAndAddMissingSupervisor(model.Approvers, model.ApproverId.Value);
        }

        if (request.TypeId == (byte)ChangeShiftType.MoveOffDay)
        {
            foreach (var detail in model.Details)
            {
                detail.DisableDates = await changeShiftService.GetDisableDateForMoveOffDayRequestAsync(model.EmployeeId, detail.OnDate, model.RequestId);
            }
        }

        ViewBag.IsHost = true;
        ViewBag.AutoApproveRequest = request.Requester.AutoApproveRequest;
        ViewBag.TypeId = request.TypeId;
        ViewBag.Shifts = await shiftService.GetAllAsync(isShowOff: true);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string model, string details, IList<IFormFile> files, string filesRemove)
    {
        var criteria = JsonConvert.DeserializeObject<EditChangeShift>(model);
        var requestDetail = JsonConvert.DeserializeObject<List<ChangeShiftDetail>>(details);
        criteria.Details = requestDetail;

        var commandResult = await changeShiftService.EditAsync(session.UserId, criteria);
        if (commandResult.IsSuccessful)
        {
            await changeShiftService.SendMailEditRequestAsync(criteria.RequestId);
            await attachFileService.UploadFileAsync(files, criteria.RequestId, $"{CurrentCompanyId}/requests");
            if (!string.IsNullOrWhiteSpace(filesRemove))
            {
                await attachFileService.DeleteFileAsync(JsonConvert.DeserializeObject<List<string>>(filesRemove), criteria.RequestId, $"{CurrentCompanyId}/requests");
            }
        }

        return Json(commandResult);
    }

    public async Task<IActionResult> Delete(long id)
    {
        await changeShiftService.SendMailDeleteRequestAsync(id);
        var commandResult = await changeShiftService.DeleteAsync(session.UserId, id);

        if (commandResult.IsSuccessful)
        {
            await attachFileService.DeleteFolderAttachFileAsync(id, $"{CurrentCompanyId}/requests");
        }

        return Json(commandResult);
    }

    public async Task<ActionResult> Details(int id)
    {
        var request = await changeShiftService.GetByIdAsync(id);
        if (request == null)
        {
            return RedirectToAction("PageNotFound", "Error");
        }

        var supervisor = await GetVerifierApprover(request.Requester.EmpId);
        var model = new ChangeShiftViewModel
        {
            Employee = request.Requester.DisplayName,
            EmployeeId = request.Requester.EmpId,
            FromDate = request.FromDate.Date,
            ToDate = request.ToDate.Date,
            VerifierId = request.Verifier?.EmpId,
            ApproverId = request.Approver?.EmpId,
            Remark = request.Remark,
            Details = await changeShiftService.GetDetailByIdAsync(id),
            Status = request.Status,
            Verifiers = supervisor.Verifiers,
            Approvers = supervisor.Approvers,
            Files = (await attachFileService.GetInfoAttachFileAsync(id, $"{CurrentCompanyId}/requests")).Select(x => x.FileName).ToList()
        };


        if (model.Verifiers.Any() && model.VerifierId.HasValue)
        {
            model.Verifiers = await employeeService.CheckAndAddMissingSupervisor(model.Verifiers, model.VerifierId.Value);
        }

        if (model.Approvers.Any() && model.ApproverId.HasValue)
        {
            model.Approvers = await employeeService.CheckAndAddMissingSupervisor(model.Approvers, model.ApproverId.Value);
        }

        ViewBag.IsHost = false;
        ViewBag.TypeId = request.TypeId;
        ViewBag.Shifts = await shiftService.GetAllAsync(isShowOff: true, empCompanyId: request.Requester.CompanyId);
        return View(model);
    }

    public async Task<ActionResult> GetChangeShiftDetail(GetWorkingCalendarForRequestParam param)
    {
        var dailySchedules = await changeShiftService.GetWorkingCalendarForRequestAsync(param);
        var details = dailySchedules.Select(x => new ChangeShiftDetail
        {
            OnDate = x.WorkDate,
            Remark = string.Empty,
            FromShiftId = GetFromShiftId(x),
            FromShift = GetFromShiftName(x)
        }).ToList();

        if (param.TypeId == (byte)ChangeShiftType.MoveOffDay)
        {
            foreach (var detail in details)
            {
                detail.DisableDates = await changeShiftService.GetDisableDateForMoveOffDayRequestAsync(param.EmployeeId, detail.OnDate, param.RequestId);
            }
        }

        ViewBag.IsHost = true;
        ViewBag.Shifts = await shiftService.GetAllAsync(isShowOff: true);
        ViewBag.TypeId = param.TypeId;

        return PartialView("_ChangeShiftDetail", details);
    }

    public async Task<IActionResult> ViewHistory(long id)
    {
        var requests = await changeShiftService.GetHistoryByIdAsync(id);
        return PartialView("_ViewHistory", requests);
    }

    private static int GetFromShiftId(DailySchedule dailySchedule)
    {
        if (dailySchedule.IsOffDay)
        {
            return Constants.OffDayShiftId;
        }

        return dailySchedule.Shift?.Id ?? 0;
    }

    private static string GetFromShiftName(DailySchedule dailySchedule)
    {
        if (dailySchedule.IsOffDay)
        {
            return Labels.Off;
        }

        return dailySchedule.Shift != null ? dailySchedule.Shift.Name : Labels.NA;
    }

    private async Task<VerifierAndApproverForRequest> GetVerifierApprover(int employeeId, RequestEntity request = null)
    {
        var supervisor = await requestService.GetVerifierAndApproverAsync(employeeId, RequestType.ChangeShift);

        if (request == null)
        {
            return supervisor;
        }
        if (request.VerifiedBy != null && supervisor.Verifiers.All(t => t.OwnerId != request.VerifiedBy.Id))
        {
            var verifier = new SupervisorNameModel
            {
                OwnerId = request.VerifiedBy.Id,
                NickName = request.VerifiedBy.NickName,
                FullName = request.VerifiedBy.FullName
            };
            supervisor.Verifiers.Add(verifier);
        }

        if (request.ApprovedBy == null || supervisor.Approvers.Any(t => t.OwnerId == request.ApprovedBy.Id))
        {
            return supervisor;
        }
        var approver = new SupervisorNameModel
        {
            OwnerId = request.ApprovedBy.Id,
            NickName = request.ApprovedBy.NickName,
            FullName = request.ApprovedBy.FullName
        };
        supervisor.Approvers.Add(approver);

        return supervisor;
    }
}