namespace HRM.Controllers.Request;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Enums;
using Fanex.Logging;
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
using Newtonsoft.Json;
using Resources;
using Services.Implement;
using Services.Interfaces;

#pragma warning disable S107 // Methods should not have too many parameters
public class LeaveController(
    ILogger logger,
    IUserControlSession session,
    ILeaveRequestService leaveRequestService,
    IRequestService requestService,
    IBaseService baseService,
    ICommonDataService commonDataService,
    IAttachFileService attachFileService,
    IEmployeeService employeeService,
    ICompanyService companyService) : BaseController(session)
#pragma warning restore S107 // Methods should not have too many parameters
{
    [HttpGet]
    public async Task<IActionResult> EmployeeView(int? year)
    {
        var tempYear = year ?? DateTimeOffset.Now.DateTime.ToClientTimeZone().Year;
        var fromDate = new DateTimeOffset(tempYear, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
        var criteria = new GetLeaveRequestByDateCriteria
        {
            EmpId = session.UserId,
            FromDate = fromDate,
            ToDate = fromDate.AddYears(1).AddDays(-1),
            CompanyId = CurrentCompanyId
        };

        var leaves = await leaveRequestService.GetByDateAsync(criteria);
        foreach (var request in leaves)
        {
            request.AttachFiles = await attachFileService.GetInfoAttachFileAsync(request.RequestId, $"{CurrentCompanyId}/requests");
            request.CreatedDate = request.CreatedDate.ToClientTimeZone();
        }
        ViewBag.Year = tempYear;
        ViewBag.EmployeeBalance = await leaveRequestService.GetLeaveBalance(session.UserId, null, year);
        return View(leaves);
    }

    public async Task<ActionResult> Create()
    {
        var supervisor = await requestService.GetVerifierAndApproverAsync(session.UserId, RequestType.Leave);
        var setting = await commonDataService.GetSystemSettingAsync();
        var today = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        var minDate = await GetEmployeeMinDate();

        var model = new LeaveRequestViewModel
        {
            LeaveTypeId = (ushort)LeaveTypes.GeneralLeave,
            Employee = EmployeeSession.DisplayName,
            EmployeeId = session.UserId,
            FromDate = today,
            ToDate = today,
            TotalDays = 1,
            LeaveTypeList = await commonDataService.GetLeaveTypeAsync(),
            EmployeeLeaveBalance = await leaveRequestService.GetLeaveBalance(session.UserId, null, null),
            Verifiers = supervisor.Verifiers,
            Approvers = supervisor.Approvers,
            MinDate = minDate,
            EnableHospitalizationLeaveBalance = setting.EnableHospitalizationLeaveBalance,
            EnableSickLeaveBalance = setting.EnableSickLeaveBalance,
            EnableLimitUseAnnualLeaveBalance = setting.EnableLimitUseAnnualLeaveBalance
        };
        ViewBag.AutoApproveRequest = EmployeeSession.AutoApproveRequest;
        ViewBag.HasOnBehalfOthersPermission = session.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
            || session.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string request, string requestDetails, IList<IFormFile> files)
    {
        var criteria = JsonConvert.DeserializeObject<InsertLeaveRequestCriteria>(request);
        var leaveRequestDetail = JsonConvert.DeserializeObject<List<LeaveRequestDetail>>(requestDetails);
        if (leaveRequestDetail is { Count: > 0 })
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
        criteria.CreatedBy = session.UserId;
        criteria.CompanyId = CurrentCompanyId;

        var startTime = DateTimeOffset.Now;
        var result = await baseService.ExecuteNonQueryAsync(criteria);
        var endTime = DateTimeOffset.Now;
        await logger.InfoAsync($"Create leave request took {(endTime - startTime).TotalSeconds} s");

        if (result.IsSuccessful)
        {
            await attachFileService.UploadFileAsync(files, criteria.RequestId, $"{CurrentCompanyId}/requests");
            if (criteria.ApproverId == null)
            {
                return new JsonResult(result);
            }

            var leaveRequest = await leaveRequestService.GetByIdAsync(criteria.RequestId);
            if (leaveRequest == null)
            {
                return new JsonResult(result);
            }

            var domain = await companyService.GetDomain(leaveRequest.Approver.CompanyId);
            leaveRequestService.SendMailAndSendLetstalkCreateRequest(criteria, domain, leaveRequest.Approver.CompanyId);
        }

        return new JsonResult(result);
    }

    public async Task<ActionResult> Update(long id)
    {
        var request = await leaveRequestService.GetByIdAsync(id);
        if (request == null
            || !Utilities.CheckUserCanEditRequest(session.UserId, request.Requester.EmpId)
            || request.Status.Id != (byte)CurrentRequestStatus.Pending)
        {
            return RedirectToAction("AccessDenied", "Error");
        }

        var model = await ConvertRequestToModel(request);
        model.DisabledDates = await leaveRequestService.GetDisabledDateAsync(new GetDisabledDateParam
        {
            EmployeeId = model.EmployeeId,
            FromDate = model.MinDate,
            ToDate = model.MaxDate,
            RequestId = model.RequestId,
            LeaveTypeId = model.LeaveTypeId
        });
        model.LeaveTypeList = await commonDataService.GetLeaveTypeAsync();
        model.EmployeeLeaveBalance = await leaveRequestService.GetLeaveBalance(session.UserId, id, null);

        var supervisor = await requestService.GetVerifierAndApproverAsync(model.EmployeeId, RequestType.Leave);
        var setting = await commonDataService.GetSystemSettingAsync();
        model.Verifiers = await employeeService.CheckAndAddMissingSupervisor(supervisor.Verifiers, model.VerifierId);
        model.Approvers = await employeeService.CheckAndAddMissingSupervisor(supervisor.Approvers, model.ApproverId);
        model.Files = (await attachFileService.GetInfoAttachFileAsync(id, $"{CurrentCompanyId}/requests")).Select(x => x.FileName).ToList();
        model.EnableHospitalizationLeaveBalance = setting.EnableHospitalizationLeaveBalance;
        model.EnableSickLeaveBalance = setting.EnableSickLeaveBalance;
        model.EnableLimitUseAnnualLeaveBalance = setting.EnableLimitUseAnnualLeaveBalance;

        ViewBag.IsView = false;
        ViewBag.AutoApproveRequest = request.Requester.AutoApproveRequest;
        ViewBag.HasOnBehalfOthersPermission = session.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
            || session.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

        return View(model);
    }

    [HttpPost]
    [SuppressMessage("CognitiveComplexity", "CA1522:AvoidExcessiveCognitiveComplexity", Justification = "Complex business logic for leave request update is necessary and well-structured")]
    [SuppressMessage("SonarQube", "S3776:Cognitive Complexity of methods should not be too high", Justification = "Complex business logic for leave request update is necessary and well-structured")]
    public async Task<IActionResult> Update(string request, string requestDetails, IList<IFormFile> files, string filesRemove)
    {
        var criteria = JsonConvert.DeserializeObject<UpdateLeaveRequestCriteria>(request);
        var leaveRequestDetail = JsonConvert.DeserializeObject<List<LeaveRequestDetail>>(requestDetails);
        criteria.RequestDetail = leaveRequestDetail;
        criteria.ModifiedBy = session.UserId;
        criteria.CompanyId = CurrentCompanyId;
        if (leaveRequestDetail is { Count: > 0 })
        {
            criteria.TotalDays = leaveRequestDetail.Where(x => x.WorkType == (byte)WorkType.Leave)
                .Sum(x => x.IsHalfDay ? Constants.HalfDayOffValue : 1);

            Parallel.ForEach(leaveRequestDetail, item =>
            {
                item.FromTime = item.IsHalfDay ? item.FromTime.ToServerTimeZone() : item.FromTime;
                item.ToTime = item.IsHalfDay ? item.ToTime.ToServerTimeZone() : item.ToTime;
            });
        }

        var startTime = DateTimeOffset.Now;
        var result = await baseService.ExecuteNonQueryAsync(criteria);
        var endTime = DateTimeOffset.Now;
        await logger.InfoAsync($"Update leave request took {(endTime - startTime).TotalSeconds} s");

        if (result.IsSuccessful)
        {
            await attachFileService.UploadFileAsync(files, criteria.RequestId, $"{CurrentCompanyId}/requests");
            if (!string.IsNullOrWhiteSpace(filesRemove))
            {
                await attachFileService.DeleteFileAsync(JsonConvert.DeserializeObject<List<string>>(filesRemove), criteria.RequestId, $"{CurrentCompanyId}/requests");
            }
            await attachFileService.UploadFileAsync(files, criteria.RequestId, $"{CurrentCompanyId}/requests");
            if (criteria.ApproverId == null)
            {
                return new JsonResult(result);
            }

            var leaveRequest = await leaveRequestService.GetByIdAsync(criteria.RequestId);
            if (leaveRequest == null)
            {
                return new JsonResult(result);
            }

            var domain = await companyService.GetDomain(leaveRequest.Approver.CompanyId);
            leaveRequestService.SendMailEditRequest(criteria, domain, leaveRequest.Approver.CompanyId);
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

        var model = await ConvertRequestToModel(request);
        model.LeaveTypeList = await commonDataService.GetLeaveTypeAsync();
        model.Files = (await attachFileService.GetInfoAttachFileAsync(id, $"{CurrentCompanyId}/requests")).Select(x => x.FileName).ToList();

        var supervisor = await requestService.GetVerifierAndApproverAsync(request.Requester.EmpId, RequestType.Leave);
        model.Verifiers = await employeeService.CheckAndAddMissingSupervisor(supervisor.Verifiers, model.VerifierId);
        model.Approvers = await employeeService.CheckAndAddMissingSupervisor(supervisor.Approvers, model.ApproverId);

        ViewBag.IsView = true;
        ViewBag.HasOnBehalfOthersPermission = session.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Read)
            || session.HasPermissionOnFunction(FunctionKey.CanOnBehalfOthers, Policy.Host);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(long id)
    {
        await leaveRequestService.SendMailDeleteRequest(id);
        var commandResult = await leaveRequestService.DeleteAsync(id);

        if (commandResult.IsSuccessful)
        {
            await attachFileService.DeleteFolderAttachFileAsync(id, $"{CurrentCompanyId}/requests");
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
            EffectedDay = toDate ?? DateTimeOffset.Now.Date.ToClientTimeZone()
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
            RequestId = param.RequestId,
            CompanyId = CurrentCompanyId
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

    private async Task<LeaveRequestViewModel> ConvertRequestToModel(LeaveRequest request)
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
        var minDate = await GetEmployeeMinDate();
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
        var fromTime = leaveRequestService.GetFromTimeHalfDay(request.LeaveDetail).ToClientTimeZone();
        var toTime = leaveRequestService.GetToTimeHalfDay(request.LeaveDetail).ToClientTimeZone();

        model.FromTime = fromTime.ToString("HH:mm");
        model.ToTime = toTime.ToString("HH:mm");
        model.FromTimeHour = fromTime.Hour;
        model.FromTimeMinute = fromTime.Minute;
        model.ToTimeHour = toTime.Hour;
        model.ToTimeMinute = toTime.Minute;
        model.IsFromTimeAfterDay = request.FromDate.Date != fromTime.Date;
        model.IsToTimeAfterDay = fromTime.Date != toTime.Date || request.ToDate.Date != toTime.Date;
    }

    private async Task<DateTime> GetEmployeeMinDate()
    {
        var today = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        var employee = await employeeService.GetEmployeeByIdAsync(session.UserId);

        if (employee != null)
        {
            var employeeJoinDate = employee.JoinedDate;

            var defaultMinDate = today.FirstDayOfMonth().AddMonths(-5);
            return employeeJoinDate.HasValue && employeeJoinDate.Value > defaultMinDate
                    ? employeeJoinDate.Value.Date
                    : defaultMinDate;
        }

        return today;
    }
}