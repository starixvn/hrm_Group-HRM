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
using HRM.Criteria.Request;
using HRM.Criteria.Request.Leave;
using HRM.Models.Request;
using HRM.Models.Request.Leave;
using HRM.Services.Interfaces.Request;
using HRM.ViewModels.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using Resources;
using Services.Implement;
using Services.Interfaces;

public class LeaveController(
    IUserControlSession userControlSession,
    ILeaveRequestService leaveRequestService,
    IRequestService requestService,
    IBaseService baseService,
    ICommonDataService commonDataService,
    IAttachFileService attachFileService) : BaseController(userControlSession)
{
    private readonly IUserControlSession userControlSession = userControlSession;
    private const string UploadFolderPath = "/file/uploads/request/";

    [HttpGet]
    public async Task<IActionResult> EmployeeView(int? year)
    {
        var tempYear = year ?? DateTimeOffset.Now.DateTime.ToClientTimeZone().Year;
        var fromDate = new DateTimeOffset(tempYear, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
        var criteria = new GetLeaveRequestByDateCriteria
        {
            EmpId = EmployeeSession.EmpId,
            FromDate = fromDate,
            ToDate = fromDate.AddYears(1).AddDays(-1),
            CompanyId = CurrentCompanyId
        };

        var leaves = await leaveRequestService.GetByDateAsync(criteria);
        foreach (var request in leaves)
        {
            request.AttachFiles = attachFileService.GetInfoAttachFile(request.RequestId, UploadFolderPath);
            request.CreatedDate = request.CreatedDate.ToClientTimeZone();
        }
        ViewBag.Year = tempYear;
        ViewBag.EmployeeBalance = await GetLeaveBalance(null, year);
        return View(leaves);
    }

    public async Task<ActionResult> Create()
    {
        var supervisor = await requestService.GetVerifierAndApproverAsync(EmployeeSession.EmpId, RequestType.Leave);
        var setting = await commonDataService.GetSystemSettingAsync();
        var today = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        var model = new LeaveRequestViewModel
        {
            LeaveTypeId = (ushort)LeaveTypes.GeneralLeave,
            Employee = EmployeeSession.DisplayName,
            EmployeeId = EmployeeSession.EmpId,
            FromDate = today,
            ToDate = today,
            TotalDays = 1,
            LeaveTypeList = await commonDataService.GetLeaveTypeAsync(),
            EmployeeLeaveBalance = await GetLeaveBalance(null, null),
            Verifiers = supervisor.Verifiers,
            Approvers = supervisor.Approvers,
            MinDate = today.FirstDayOfMonth().AddMonths(-5),
            EnableHospitalizationLeaveBalance = setting.EnableHospitalizationLeaveBalance,
            EnableSickLeaveBalance = setting.EnableSickLeaveBalance
        };
        ViewBag.IsTopManager = EmployeeSession.IsTopManager;
        ViewBag.HasOnBehalfOthersPermission = userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                                              || userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string request, string requestDetails, IList<IFormFile> files)
    {
        var criteria = JsonConvert.DeserializeObject<InsertLeaveRequestCriteria>(request);
        var leaveRequestDetail = JsonConvert.DeserializeObject<List<LeaveRequestDetail>>(requestDetails);
        if (leaveRequestDetail != null && leaveRequestDetail.Any())
        {
            criteria.TotalDays = leaveRequestDetail.Where(x => x.WorkType == (byte)WorkType.Leave)
                .Sum(x => x.IsHalfDay ? Constants.HalfDayOffValue : 1);

            Parallel.ForEach(leaveRequestDetail, item =>
            {
                item.FromTime = item.IsHalfDay ? item.FromTime.ToServerTimeZone() : item.FromTime;
                item.ToTime = item.IsHalfDay ? item.ToTime.ToServerTimeZone() : item.ToTime;
            });
        }

        criteria.RequestDetail = leaveRequestDetail;
        criteria.CreatedBy = EmployeeSession.EmpId;
        criteria.CompanyId = CurrentCompanyId;

        var result = await baseService.ExecuteNonQueryAsync(criteria);
        if (result.IsSuccessful)
        {
            leaveRequestService.SendMailAndSendLetstalkCreateRequest(criteria);
            attachFileService.UploadFile(files, criteria.RequestId, UploadFolderPath);
        }

        return new JsonResult(result);
    }

    public async Task<ActionResult> Update(long id)
    {
        var request = await leaveRequestService.GetByIdAsync(id);
        if (request == null
            || !Utilities.CheckUserCanEditRequest(EmployeeSession.EmpId, request.Requester.EmpId)
            || request.Status.Id != (byte)CurrentRequestStatus.Pending)
        {
            return RedirectToAction("AccessDenied", "Error");
        }

        var model = ConvertRequestToModel(request);
        model.DisabledDates = await leaveRequestService.GetDisabledDateAsync(new GetDisabledDateParam
        {
            EmployeeId = model.EmployeeId,
            FromDate = model.MinDate,
            ToDate = model.MaxDate,
            RequestId = model.RequestId,
            LeaveTypeId = model.LeaveTypeId
        });
        model.LeaveTypeList = await commonDataService.GetLeaveTypeAsync();
        model.EmployeeLeaveBalance = await GetLeaveBalance(id, null);

        var supervisor = await requestService.GetVerifierAndApproverAsync(model.EmployeeId, RequestType.Leave);
        var setting = await commonDataService.GetSystemSettingAsync();
        model.Verifiers = supervisor.Verifiers;
        model.Approvers = supervisor.Approvers;
        model.Files = attachFileService.GetAttachFile(id, UploadFolderPath);
        model.EnableHospitalizationLeaveBalance = setting.EnableHospitalizationLeaveBalance;
        model.EnableSickLeaveBalance = setting.EnableSickLeaveBalance;

        ViewBag.IsView = false;
        ViewBag.IsTopManager = request.Requester.IsTopManager;
        ViewBag.HasOnBehalfOthersPermission = userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                                              || userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(string request, string requestDetails, IList<IFormFile> files, string filesRemove)
    {
        var criteria = JsonConvert.DeserializeObject<UpdateLeaveRequestCriteria>(request);
        var leaveRequestDetail = JsonConvert.DeserializeObject<List<LeaveRequestDetail>>(requestDetails);
        criteria.RequestDetail = leaveRequestDetail;
        if (leaveRequestDetail != null && leaveRequestDetail.Any())
        {
            criteria.TotalDays = leaveRequestDetail.Where(x => x.WorkType == (byte)WorkType.Leave)
                .Sum(x => x.IsHalfDay ? Constants.HalfDayOffValue : 1);

            Parallel.ForEach(leaveRequestDetail, item =>
            {
                item.FromTime = item.IsHalfDay ? item.FromTime.ToServerTimeZone() : item.FromTime;
                item.ToTime = item.IsHalfDay ? item.ToTime.ToServerTimeZone() : item.ToTime;
            });
        }

        criteria.ModifiedBy = EmployeeSession.EmpId;

        var result = await baseService.ExecuteNonQueryAsync(criteria);
        if (result.IsSuccessful)
        {
            leaveRequestService.SendMailEditRequest(criteria);
            attachFileService.UploadFile(files, criteria.RequestId, UploadFolderPath);
            if (!string.IsNullOrWhiteSpace(filesRemove))
            {
                attachFileService.DeleteFile(JsonConvert.DeserializeObject<List<string>>(filesRemove), criteria.RequestId, UploadFolderPath);
            }
        }

        return Json(result);
    }

    public async Task<ActionResult> Detail(long id)
    {
        var request = await leaveRequestService.GetByIdAsync(id);
        if (request == null)
        {
            return RedirectToAction("PageNotFound", "Error");
        }

        var model = ConvertRequestToModel(request);
        model.LeaveTypeList = await commonDataService.GetLeaveTypeAsync();
        model.Files = attachFileService.GetAttachFile(id, UploadFolderPath);

        var supervisor = await requestService.GetVerifierAndApproverAsync(request.Requester.EmpId, RequestType.Leave);
        model.Verifiers = supervisor.Verifiers;
        model.Approvers = supervisor.Approvers;

        ViewBag.IsView = true;
        ViewBag.HasOnBehalfOthersPermission = userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
                                              || userControlSession.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(long id)
    {
        await leaveRequestService.SendMailDeleteRequest(id);
        var commandResult = await leaveRequestService.DeleteAsync(id);

        if (commandResult.IsSuccessful)
        {
            attachFileService.DeleteFolderAttachFile(id, UploadFolderPath);
        }

        return Json(commandResult);
    }

    public async Task<ActionResult> ViewHistory(long id)
    {
        var requests = await leaveRequestService.GetHistoryByIdAsync(id);
        return PartialView("_ViewHistory", requests);
    }

    public async Task<IActionResult> GetLeaveBalance(int employeeId, long? requestId, DateTime? toDate)
    {
        var criteria = new GetLeaveBalanceCriteria
        {
            EmpId = employeeId,
            RequestId = requestId == -1 ? null : requestId,
            EffectedDay = toDate ?? DateTimeOffset.Now.Date
        };
        var employeeLeaveBalance = await leaveRequestService.GetLeaveBalanceAsync(criteria);
        return new JsonResult(employeeLeaveBalance);
    }

    public async Task<ActionResult> LeaveRequestDetail(int employeeId, DateTime fromDate, DateTime toDate, long requestId = -1)
    {
        var calendars = await leaveRequestService.GetEmployeeCalendarAsync(employeeId, fromDate, toDate);
        var leaveRequestDetail = requestId != -1
            ? await leaveRequestService.GetLeaveRequestDetailByIdAsync(requestId)
            : [];

        Parallel.ForEach(leaveRequestDetail, item =>
        {
            item.FromTime = item.IsHalfDay ? item.FromTime.ToClientTimeZone() : item.FromTime;
            item.ToTime = item.IsHalfDay ? item.ToTime.ToClientTimeZone() : item.FromTime;
        });

        leaveRequestDetail = ConvertToLeaveRequestDetail(calendars, leaveRequestDetail, fromDate, toDate);
        var leaveTypes = await commonDataService.GetLeaveTypeAsync();
        var model = new LeaveDetailViewModel
        {
            LeaveRequestDetail = leaveRequestDetail,
            LeaveTypeList = leaveTypes.Where(x => x.Id != (int)LeaveTypes.GeneralLeave
                                                  && x.Id != (int)LeaveTypes.KeepInViewByHour
                                                  && x.Id != (int)LeaveTypes.BusinessDuringWorking).ToList()
        };

        return View(model);
    }

    public async Task<IActionResult> GetDisableDate(int leaveTypeId, int employeeId, DateTime date, long? requestId = null)
    {
        var fromDate = new DateTimeOffset(date.AddMonths(-5).Year, date.AddMonths(-5).Month, 1, 0, 0, 0, TimeSpan.Zero).Date;
        var tempDate = date.AddMonths(Constants.NumberMonthsInFutureCanLeaveRequest);
        var toDate = new DateTimeOffset(tempDate.Year, tempDate.Month, DateTime.DaysInMonth(tempDate.Year, tempDate.Month), 0, 0, 0, TimeSpan.Zero).Date;
        var param = new GetDisabledDateParam
        {
            LeaveTypeId = leaveTypeId,
            RequestId = requestId,
            EmployeeId = employeeId,
            FromDate = fromDate,
            ToDate = toDate
        };
        var dates = await leaveRequestService.GetDisabledDateAsync(param);
        return Json(dates.Select(t => t.Date.ToString("yyyy/MM/dd")));
    }

    public async Task<IActionResult> GetTotalLeaveDay(GetDisabledDateParam param)
    {
        var criteria = new GetTotalLeaveDayCriteria
        {
            LeaveTypeId = param.LeaveTypeId,
            EmployeeId = param.EmployeeId,
            FromDate = param.FromDate,
            ToDate = param.ToDate,
            RequestId = param.RequestId
        };
        var dates = await baseService.GetDataAsync<DateTime>(criteria);
        var totalDays = (param.ToDate - param.FromDate).Days + 1 - dates.Count();
        return Json(totalDays);
    }

    private static List<LeaveRequestDetail> ConvertToLeaveRequestDetail(
        IEnumerable<Models.ScheduleArrange.DailySchedule> calendars,
        IList<LeaveRequestDetail> leaveDetails,
        DateTime fromDate,
        DateTime toDate)
    {
        var details = new List<LeaveRequestDetail>();
        if (fromDate <= toDate)
        {
            var numberDays = (toDate - fromDate).Days + 1;
            DateTime tempDate;

            for (var i = 0; i < numberDays; i++)
            {
                tempDate = fromDate.AddDays(i);
                var tempLeaveDetailModels = leaveDetails.Where(t => t.OnDate == tempDate);
                var calendar = calendars.FirstOrDefault(t => t.WorkDate.Date == tempDate);

                if (calendar != null)
                {
                    AddLeaveRequestExistCalendar(details, tempLeaveDetailModels, calendar);
                }
                else
                {
                    AddLeaveRequestNotExistCalendar(details, tempLeaveDetailModels, tempDate);
                }
            }
        }

        return details.OrderBy(t => t.OnDate).ThenBy(t => t.FromTime).ToList();
    }

    private static void AddLeaveRequestExistCalendar(
        List<LeaveRequestDetail> leaveRequestDetailOutput,
        IEnumerable<LeaveRequestDetail> leaveDetails,
        Models.ScheduleArrange.DailySchedule calendar)
    {
        var tempScheduleStatus = string.Empty;
        LeaveRequestDetail tempLeaveDetailModel;
        var isOffOrHoliday = calendar.IsOffDay || calendar.IsHoliday;

        if (!calendar.IsLeaveDay)
        {
            tempScheduleStatus = Labels.NA;
        }

        if (calendar.IsOffDay)
        {
            tempScheduleStatus = Labels.OffDay;
        }

        if (calendar.IsHoliday)
        {
            tempScheduleStatus = Labels.Holiday;
        }

        if (calendar.Shift != null)
        {
            tempScheduleStatus = calendar.Shift.Name;
        }

        if (leaveDetails != null && leaveDetails.Any())
        {
            foreach (var leaveDetail in leaveDetails)
            {
                tempLeaveDetailModel = leaveDetail;
                tempLeaveDetailModel.IsOffOrHoliday = isOffOrHoliday;
                tempLeaveDetailModel.ScheduleStatus = tempScheduleStatus;
                tempLeaveDetailModel.IsFromAfterday = leaveDetail.FromTime.Date > leaveDetail.OnDate.Date;
                tempLeaveDetailModel.IsToAfterday = leaveDetail.ToTime.Date > leaveDetail.OnDate.Date;
                leaveRequestDetailOutput.Add(tempLeaveDetailModel);
            }
        }
        else
        {
            tempLeaveDetailModel = new LeaveRequestDetail
            {
                OnDate = calendar.WorkDate,
                ScheduleStatus = tempScheduleStatus,
                IsOffOrHoliday = isOffOrHoliday,
                LeaveTypeId = (ushort)LeaveTypes.AnnualLeave,
                WorkType = (byte)WorkType.Leave
            };
            leaveRequestDetailOutput.Add(tempLeaveDetailModel);
        }
    }

    private static void AddLeaveRequestNotExistCalendar(
        List<LeaveRequestDetail> leaveRequestDetailOutput,
        IEnumerable<LeaveRequestDetail> tempLeaveDetailModels,
        DateTime workDate)
    {
        var tempScheduleStatus = Labels.NA;
        LeaveRequestDetail tempLeaveDetailModel;

        if (tempLeaveDetailModels != null && tempLeaveDetailModels.Any())
        {
            foreach (var leaveDetail in tempLeaveDetailModels)
            {
                tempLeaveDetailModel = leaveDetail;
                tempLeaveDetailModel.ScheduleStatus = tempScheduleStatus;
                tempLeaveDetailModel.IsFromAfterday = leaveDetail.FromTime.Date > leaveDetail.OnDate.Date;
                tempLeaveDetailModel.IsToAfterday = leaveDetail.ToTime.Date > leaveDetail.OnDate.Date;
                leaveRequestDetailOutput.Add(tempLeaveDetailModel);
            }
        }
        else
        {
            tempLeaveDetailModel = new LeaveRequestDetail
            {
                OnDate = workDate,
                ScheduleStatus = Labels.NA,
                LeaveTypeId = (ushort)LeaveTypes.AnnualLeave,
                WorkType = (byte)WorkType.Leave
            };
            leaveRequestDetailOutput.Add(tempLeaveDetailModel);
        }
    }

    private LeaveRequestViewModel ConvertRequestToModel(LeaveRequest request)
    {
        var model = new LeaveRequestViewModel
        {
            RequestId = request.RequestId,
            LeaveTypeId = request.LeaveType.Id,
            Employee = request.Requester.DisplayName,
            EmployeeId = request.Requester.EmpId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            TotalDays = request.TotalDays,
            HalfDayOff = request.IsHalfDay,
            IsHospitalApplication = request.IsHospitalApplication,
            VerifierId = request.Verifier?.EmpId ?? 0,
            ApproverId = request.Approver?.EmpId ?? 0,
            RefNo = request.RefNO,
            Remark = request.Remark,
            Status = request.Status
        };

        if (request.IsHalfDay && request.LeaveType.Id != (short)LeaveTypes.GeneralLeave)
        {
            GetTimeForHalfDayRequest(request, model);
        }

        var today = DateTimeOffset.Now.Date;
        var minDate = today.FirstDayOfMonth().AddMonths(-5);
        var maxDate = today.FirstDayOfMonth().AddMonths(Constants.NumberMonthsInFutureCanLeaveRequest).AddDays(-1);
        if (model.LeaveTypeId == (ushort)LeaveTypes.MaternityLeave)
        {
            // rule: user can make maternity leave for 6 months.
            const byte ExtraMonthsCanRequest = 2;
            maxDate = maxDate.AddMonths(ExtraMonthsCanRequest);
        }

        model.MinDate = minDate > model.FromDate ? model.FromDate : minDate;
        model.MaxDate = maxDate;

        return model;
    }

    private void GetTimeForHalfDayRequest(LeaveRequest request, LeaveRequestViewModel model)
    {
        var fromTime = leaveRequestService.GetFromTimeHalfDay(request.LeaveDetail);
        var toTime = leaveRequestService.GetToTimeHalfDay(request.LeaveDetail);

        model.FromTime = fromTime.ToString("HH:mm");
        model.ToTime = toTime.ToString("HH:mm");
        model.FromTimeHour = fromTime.Hour;
        model.FromTimeMinute = fromTime.Minute;
        model.ToTimeHour = toTime.Hour;
        model.ToTimeMinute = toTime.Minute;
        model.IsFromTimeAfterDay = request.FromDate.Date != fromTime.Date;
        model.IsToTimeAfterDay = fromTime.Date != toTime.Date || request.ToDate.Date != toTime.Date;
    }

    private async Task<EmployeeLeaveBalance> GetLeaveBalance(long? requestId, int? year)
    {
        var criteria = new GetLeaveBalanceCriteria
        {
            EmpId = EmployeeSession.EmpId,
            RequestId = requestId,
            EffectedDay = year.HasValue ? new DateTimeOffset(year.Value, 1, 1, 0, 0, 0, TimeSpan.Zero).Date : DateTimeOffset.Now.Date
        };
        return await leaveRequestService.GetLeaveBalanceAsync(criteria);
    }
}