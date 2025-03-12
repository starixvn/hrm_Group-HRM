using HRM.Services.ExportExcel;
using HRM.Services.Implement;
using HRM.Services.Interfaces;

namespace HRM.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using Core;
using Core.Constants;
using Core.Enums;
using Core.Filters;
using HRM.Core.Models;
using Criteria.ScheduleArrange;
using Models;
using Models.ScheduleArrange;
using ViewModels.ScheduleArrange;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class ScheduleArrangeController(
    AppSettings appSettings,
    IUserControlSession userControlSession,
    IRoleService roleService,
    IScheduleArrangeService scheduleArrangeService,
    ISyncWorkingShiftService syncWorkingShiftService,
    IExportExcelScheduleService exportExcelService) : BaseController(userControlSession)
{
    private const string DateFormat = "dd-MMM-yyyy";

    [HttpGet]
    [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
    public async Task<IActionResult> Index()
    {
        ViewBag.HasPermissionToViewHoliday = userControlSession.HasPermissionOnFunction(FunctionKey.Holiday, Policy.Read);
        ViewBag.HasPermissionToViewShift = userControlSession.HasPermissionOnFunction(FunctionKey.ShiftManagement, Policy.Read);
        ViewBag.EnableSyncWorkingShift = appSettings.EnableManualSyncWorkingShift;
        ViewBag.IsSystemAdmin = (await roleService.GetByUserIdAsync(userControlSession.UserId))
            .Any(r => r.RoleId is (short)RoleSystem.HR or (short)RoleSystem.SystemAdmin);
        return View();
    }

    [AllowAnonymous]
    public IActionResult IFrame()
    {
        return View();
    }

    [AllowAnonymous]
    public async Task<IActionResult> GetEmployees(GetEmployeeFilter filter)
    {
        filter.ActorId = EmployeeSession.EmpId;
        var employees = await scheduleArrangeService.GetEmployeesAsync(filter);

        return Json(employees);
    }

    [AllowAnonymous]
    public async Task<IActionResult> GetWeeklySchedule(WeeklyScheduleFilter filter)
    {
        filter.ActorId = EmployeeSession.EmpId;
        var weeklySchedules = await scheduleArrangeService.GetWeeklyScheduleAsync(filter);
        var schedules = weeklySchedules.GroupBy(x => new { x.Team.Id, x.Team.Code, x.Team.Name })
            .Select(x => new WeeklyScheduleViewModel
            {
                TeamId = x.Key.Id,
                Name = x.Key.Name,
                Code = x.Key.Code,
                Members = x
            });

        var isHost = CheckPermission(filter.FromDate);
        var data = new { isHost, schedules };

        return Json(data);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
    public async Task<IActionResult> EditWeekly(
        DateTime fromDate,
        DateTime toDate,
        IEnumerable<EditDailySchedule> weeklySchedules,
        IEnumerable<EditDefaultShift> defaultShifts)
    {
        var commandResult = await scheduleArrangeService.EditWeeklyAsync(
            EmployeeSession.EmpId,
            fromDate,
            toDate,
            weeklySchedules,
            defaultShifts);

        return Json(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
    public async Task<IActionResult> SyncWorkingShift(DateTime fromDate, DateTime toDate, string employeeIds)
    {
        var command = new SyncWorkingShiftCommand
        {
            ActorId = EmployeeSession.EmpId,
            FromDate = fromDate,
            ToDate = toDate,
            EmployeeIds = employeeIds
        };
        var commandResult = await syncWorkingShiftService.SyncWorkingShiftAsync(command);

        return Json(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
    public async Task<IActionResult> EditRemark(EditRemark model)
    {
        var commandResult = await scheduleArrangeService.EditRemarkAsync(EmployeeSession.EmpId, model);

        return Json(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
    public async Task<IActionResult> EditDisplayOrder(IEnumerable<EditDisplayOrder> displayOrders)
    {
        var commandResult = await scheduleArrangeService.EditDisplayOrderAsync(EmployeeSession.EmpId, displayOrders);

        return Json(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
    public async Task<IActionResult> EditTeamOrder(IEnumerable<EditTeamOrder> displayOrders)
    {
        var commandResult = await scheduleArrangeService.EditTeamOrderAsync(EmployeeSession.EmpId, displayOrders);

        return Json(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
    public async Task<IActionResult> EditWeeklyDefaultShift(WeeklyDefaultShift weeklyDefaultShift, bool isUsedWeeklyDefaultShift)
    {
        var commandResult = await scheduleArrangeService.EditWeeklyDefaultShiftAsync(EmployeeSession.EmpId, weeklyDefaultShift, isUsedWeeklyDefaultShift);

        return Json(commandResult);
    }

    [HttpGet]
    [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
    public async Task<IActionResult> ExportExcel(WeeklyScheduleFilter filter)
    {
        filter.ActorId = EmployeeSession.EmpId;
        var weeklySchedules = await scheduleArrangeService.GetWeeklyScheduleAsync(filter);
        var bytes = exportExcelService.Export(filter, weeklySchedules);

        var fileName = $"Working shift {filter.FromDate.ToString(DateFormat)} to {filter.ToDate.ToString(DateFormat)}.xlsx";
        return File(bytes, MimeType.XLSX, fileName);
    }

    [HttpGet]
    [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
    public async Task<IActionResult> ExportExcelDriverPickup(WeeklyScheduleFilter filter)
    {
        var actor = new Actor
        {
            ActorId = EmployeeSession.EmpId,
            Nickname = EmployeeSession.Nickname,
            FullName = EmployeeSession.FullName
        };
        filter.ActorId = EmployeeSession.EmpId;
        var weeklySchedules = await scheduleArrangeService.GetWeeklyScheduleAsync(filter);
        var bytes = exportExcelService.ExportDriverPickup(actor, filter, weeklySchedules);

        var fileName = $"Schedule for Driver {filter.FromDate.ToString(DateFormat)} to {filter.ToDate.ToString(DateFormat)}.xlsx";
        return File(bytes, MimeType.XLSX, fileName);
    }

    private bool CheckPermission(DateTime fromDate)
    {
        var firstDayOfWeek = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date.FirstDayOfWeek();

        bool isHost;
        if (userControlSession.HasPermissionOnFunction(FunctionKey.EditScheduleUnlimited, Policy.Host))
        {
            isHost = true;
        }
        else if (userControlSession.HasPermissionOnFunction(FunctionKey.EditScheduleFromCurrentWeek, Policy.Host))
        {
            isHost = fromDate.Date >= firstDayOfWeek;
        }
        else if (userControlSession.HasPermissionOnFunction(FunctionKey.EditScheduleFromNextWeek, Policy.Host))
        {
            const int nextWeek = 7;
            isHost = fromDate.Date >= firstDayOfWeek.AddDays(nextWeek);
        }
        else
        {
            isHost = false;
        }

        return isHost;
    }
}