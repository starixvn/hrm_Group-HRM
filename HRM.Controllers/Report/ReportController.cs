namespace HRM.Controllers.Report;

using System;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Enums;
using Core.Filters;
using Criteria.Supervisor;
using Criteria.Time;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Criteria.Report;
using HRM.ViewModels.Report;
using Microsoft.AspNetCore.Mvc;
using Services.Implement;
using Services.Interfaces;
using ViewModels;
using ViewModels.AnnualLeave;

public class ReportController(
    IUserControlSession userControlSession,
    IReportService reportService,
    ICommonDataService commonDataService,
    ITeamService teamService) : BaseController(userControlSession)
{
    private readonly IUserControlSession userControlSession = userControlSession;

    [HrmPermission(Policy.Read, FunctionKey.ReportHome)]
    public ActionResult Index()
    {
        var functions = userControlSession.Functions.Where(x => x.IsRead || x.IsHost);
        return View(functions);
    }

    [HttpGet]
    public async Task<IActionResult> Timesheet()
    {
        var model = new TimesheetViewModel
        {
            Year = DateTimeOffset.Now.DateTime.ToClientTimeZone().Year,
            EmpId = EmployeeSession.EmpId,
            IsHavePrivilegeCreateCheckInOutRecord = await IsPrivilegeCreateCheckInOut()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> TimesheetDetails(TimesheetViewModel model)
    {
        var today = DateTimeOffset.Now.DateTime.ToClientTimeZone();
        var fromDate = new DateTimeOffset(model.Year, model.Month, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
        var toDate = today.Month == model.Month && today.Year == model.Year
            ? today.Date : fromDate.AddMonths(1).AddDays(-1);

        var criteria = new GetEmployeeAttendanceCriteria
        {
            Fromtime = fromDate,
            Totime = toDate,
            AttendanceType = 0, //show All
            EmpId = model.EmpId,
            ExcludeUserDelete = 1,
            CompanyId = CurrentCompanyId
        };

        var timesheets = await reportService.GetEmployeeAttendanceReportAsync(criteria);
        foreach (var timesheet in timesheets)
        {
            if (timesheet.InFixed != null)
            {
                timesheet.InFixed = timesheet.InFixed.Value.ToClientTimeZone();
            }

            if (timesheet.OutFixed != null)
            {
                timesheet.OutFixed = timesheet.OutFixed.Value.ToClientTimeZone();
            }
        }

        return PartialView("_TimesheetDetails", timesheets);
    }

    public async Task<ViewResult> TimeRecord(TimeRecordViewModel model)
    {
        model.CurrentPage = Constants.CurrentPage;
        model.ItemsPerPage = Constants.ItemsPerPage;
        var supervisors = await teamService.GetBySupervisorAsync(EmployeeSession.EmpId);
        model.TeamIds = string.Join(",", supervisors.Select(x => x.Id));

        ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.TimeRecord, Policy.Read)
            || userControlSession.HasPermissionOnFunction(FunctionKey.TimeRecord, Policy.Host);

        return View(model);
    }

    [HttpPost]
    public async Task<PartialViewResult> TimeRecordDetail(TimeRecordViewModel model)
    {
        int? employeeId = null;
        if (!userControlSession.HasPermissionOnFunction(FunctionKey.TimeRecord, Policy.Read)
            && !userControlSession.HasPermissionOnFunction(FunctionKey.TimeRecord, Policy.Host))
        {
            employeeId = EmployeeSession.EmpId;
        }
        var criteria = new GetTimeRecordCriteria
        {
            EmpId = model.EmployeeId == 0 ? employeeId : model.EmployeeId,
            TeamIds = model.TeamIds,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            IsLocal = model.IsLocal,
            PageIndex = model.CurrentPage - 1,
            ItemsPerPage = model.ItemsPerPage,
            SupervisorId = EmployeeSession.EmpId
        };
        var timeRecords = await reportService.GetTimeRecords(criteria);
        ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.TimeRecord, Policy.Host);

        var pagerOption = new PagerOptionModel
        {
            CurrentPage = model.CurrentPage,
            ItemsPerPage = model.ItemsPerPage,
            TotalItem = criteria.TotalItem ?? 0
        };
        ViewBag.PagerOption = pagerOption;

        return PartialView("_TimeRecordDetail", timeRecords);
    }

    public ActionResult WeekendLeaveSummary(WeekendLeaveViewModel model)
    {
        var currentYear = DateTimeOffset.Now.DateTime.ToClientTimeZone().Year;
        model.FromDate = new DateTimeOffset(currentYear, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
        model.ToDate = model.FromDate.AddYears(1).AddDays(-1);
        model.DateRange = Utilities.GetDateRangeByYear();

        return View("WeekendLeaveSummary", model);
    }

    public async Task<IActionResult> PartialListWeekendLeaveSummary(WeekendLeaveViewModel model)
    {
        var criteria = new GetWeekendLeaveSummaryCriteria
        {
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamId = model.TeamId,
            IsLocal = model.IsLocal,
            EmployeeId = model.EmployeeId,
            MinTotalLeave = model.MinTotalLeave,
            SupervisorId = EmployeeSession.EmpId
        };

        var weekendLeaves = await reportService.GetWeekendLeaveSummaryAsync(criteria);

        return PartialView("_ListWeekendLeaveSummary", weekendLeaves);
    }

    public ActionResult WeekendLeaveDetail(WeekendLeaveViewModel model)
    {
        var today = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        model.FromDate = new DateTimeOffset(today.Year, 1, 1, 0, 0, 0, TimeSpan.Zero).Date;
        model.ToDate = today;
        model.DateRange = Utilities.GetDateRangeByYear();

        return View("WeekendLeaveDetail", model);
    }

    public async Task<IActionResult> PartialListWeekendLeaveDetail(WeekendLeaveViewModel model)
    {
        var criteria = new GetWeekendLeaveDetailCriteria
        {
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamId = model.TeamId,
            IsLocal = model.IsLocal,
            EmployeeId = model.EmployeeId,
            LeaveTypeId = model.LeaveTypeId,
            SupervisorId = EmployeeSession.EmpId
        };

        var weekendLeaveDetails = await reportService.GetWeekendLeaveDetailAsync(criteria);

        return PartialView("_ListWeekendLeaveDetail", weekendLeaveDetails);
    }

    [HrmPermission(Policy.Read, FunctionKey.UnpaidLeaveSummary)]
    public async Task<IActionResult> UnpaidLeaveSummary()
    {
        var teams = await teamService.GetAllAsync();
        var model = new LeaveSummaryViewModel
        {
            Year = DateTimeOffset.Now.DateTime.ToClientTimeZone().Year,
            EmployeeId = -1,
            TeamIds = string.Join(",", teams.Select(x => x.Id))
        };

        ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.CanFilterUnpaidLeaveSummary, Policy.Read)
            || userControlSession.HasPermissionOnFunction(FunctionKey.CanFilterUnpaidLeaveSummary, Policy.Host);

        return View(model);
    }

    [HrmPermission(Policy.Read, FunctionKey.UnpaidLeaveSummary)]
    public async Task<IActionResult> GetUnpaidLeaveSummary(LeaveSummaryViewModel model)
    {
        var criteria = new GetUnpaidLeaveCriteria
        {
            Year = model.Year,
            EmpId = model.EmployeeId > 0 ? new int?(model.EmployeeId) : null,
            TeamIds = model.TeamIds,
            IsLocal = (byte?)model.Category,
            SupervisorId = EmployeeSession.EmpId,
            LeaveType = model.LeaveType,
            CompanyId = CurrentCompanyId
        };

        var leaveTypeIds = !string.IsNullOrWhiteSpace(model.LeaveType) ? model.LeaveType.Split(',').ToList() : [];
        var leaveTypes = await commonDataService.GetLeaveTypeAsync();
        var leaveType = string.Join(", ", leaveTypes.Where(x => leaveTypeIds.Exists(y => x.Id.ToString() == y)).Select(x => x.Name));
        ViewBag.HeaderTitle = $"{leaveType} {model.Year}";

        var leaveSummaries = await reportService.GetUnpaidLeaveAsync(criteria);
        model.LeaveSumaries = model.Period switch
        {
            "1" => leaveSummaries.Where(x => x.Jan > 0 || x.Feb > 0 || x.Mar > 0 || x.Apr > 0 || x.May > 0 || x.Jun > 0)
                .ToList(),
            "2" => leaveSummaries.Where(x => x.Jul > 0 || x.Aug > 0 || x.Sep > 0 || x.Oct > 0 || x.Nov > 0 || x.Dec > 0)
                .ToList(),
            _ => leaveSummaries
        };

        return PartialView("_UnpaidLeaveSummaryDetails", model);
    }

    public async Task<ViewResult> EmployeeSupervisor()
    {
        var model = new EmployeeAndTeamSupervisorViewModel();
        var teams = await teamService.GetAllAsync();
        model.TeamIds ??= string.Join(",", teams.Select(x => x.Id));

        return View("EmployeeSupervisor", model);
    }

    public async Task<PartialViewResult> EmployeeSupervisorDetails(EmployeeAndTeamSupervisorViewModel model)
    {
        var criteria = new GetSupervisorReportCriteria
        {
            EmployeeId = model.EmployeeId,
            TeamIds = model.TeamIds,
            IsLocal = model.IsLocal,
            ActionType = model.ActionType
        };
        var supervisors = await reportService.GetSupervisorReport(criteria);

        return PartialView("_EmployeeSupervisorDetails", supervisors);
    }

    public async Task<ViewResult> TeamSupervisor()
    {
        var model = new EmployeeAndTeamSupervisorViewModel();
        var teams = await teamService.GetAllAsync();
        model.TeamIds ??= string.Join(",", teams.Select(x => x.Id));

        return View("TeamSupervisor", model);
    }

    public async Task<PartialViewResult> TeamSupervisorDetails(EmployeeAndTeamSupervisorViewModel model)
    {
        var criteria = new GetTeamSupervisorCriteria
        {
            TeamIds = model.TeamIds
        };
        var supervisors = await reportService.GetTeamSupervisor(criteria);

        return PartialView("_TeamSupervisorDetails", supervisors);
    }

    public async Task<IActionResult> GetUnpaidLeaveType()
    {
        var leaveTypes = await commonDataService.GetLeaveTypeAsync();
        var multiSelectViewModel = leaveTypes.Where(x => x.Id is (int)LeaveTypes.MaternityLeave
            or (int)LeaveTypes.PaternityLeave
            or (int)LeaveTypes.UnpaidLeave
            or (int)LeaveTypes.UnpaidLeaveInsurance).Select(u => new MultiSelectViewModel
            {
                Id = u.Id,
                Text = u.Name,
                IsLeaf = true
            });

        return Json(multiSelectViewModel);
    }

    private async Task<bool> IsPrivilegeCreateCheckInOut()
    {
        var criteria = new GetPrivilegeCreateCheckInOutRecordCriteria
        {
            EmpId = EmployeeSession.EmpId
        };

        var isPrivilegeCreateCheckInOut = await reportService.GetIsPrivilegeCreateCheckInOutAsync(criteria);
        return isPrivilegeCreateCheckInOut;
    }
}