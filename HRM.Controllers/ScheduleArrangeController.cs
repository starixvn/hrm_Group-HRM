namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;
    using HRM.Controllers;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Filters;
    using HRM.Core.Models;
    using HRM.Criteria.ScheduleArrange;
    using HRM.Models;
    using HRM.Models.ScheduleArrange;
    using HRM.Services;
    using HRM.ViewModels.ScheduleArrange;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    public class ScheduleArrangeController(
        IUserControlSession userControlSession,
        IRoleService roleService,
        IScheduleArrangeService scheduleArrangeService,
        ISyncWorkingShiftService syncWorkingShiftService,
        IExportExcelScheduleService exportExcelService,
        IConfiguration configuration) : BaseController(userControlSession)
    {
        private const string DateFormat = "dd-MMM-yyyy";

        [HttpGet]
        [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult Index()
        {
            ViewBag.HasPermissionToViewHoliday = userControlSession.HasPermissionOnFunction(FunctionKey.Holiday, Policy.Read);
            ViewBag.HasPermissionToViewShift = userControlSession.HasPermissionOnFunction(FunctionKey.ShiftManagement, Policy.Read);
            ViewBag.EnableSyncWorkingShift = configuration.GetValue<bool>("EnableManualSyncWorkingShift");
            ViewBag.SiteId = configuration.GetValue<int>("GO.UserControl.Client:SiteId");
            ViewBag.IsSystemAdmin = roleService.GetByUserId(userControlSession.UserId)
                .Any(r => r.RoleId == (short)RoleSystem.HR || r.RoleId == (short)RoleSystem.SystemAdmin);
            return View();
        }

        [AllowAnonymous]
        public ActionResult IFrame()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetEmployees(GetEmployeeFilter filter)
        {
            filter.ActorId = EmployeeSession.EmpId;
            var employees = await scheduleArrangeService.GetEmployeesAsync(filter);

            return Json(employees) ;
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

            bool isHost = CheckPermission(filter.FromDate);
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
        public async Task<ActionResult> SyncWorkingShift(DateTime fromDate, DateTime toDate, string employeeIds)
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
            byte[] bytes = exportExcelService.Export(filter, weeklySchedules);

            var fileName = $"Working shift {filter.FromDate.ToString(DateFormat)} to {filter.ToDate.ToString(DateFormat)}.xlsx";
            return File(bytes, MimeType.XLSX, fileName);
        }

        [HttpGet]
        [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public async Task<IActionResult> ExportExcelDriverPickup(WeeklyScheduleFilter filter)
        {
            Actor actor = new Actor
            {
                ActorId = EmployeeSession.EmpId,
                Nickname = EmployeeSession.Nickname,
                FullName = EmployeeSession.FullName
            };
            filter.ActorId = EmployeeSession.EmpId;
            var weeklySchedules = await scheduleArrangeService.GetWeeklyScheduleAsync(filter);
            byte[] bytes = exportExcelService.ExportDriverPickup(actor, filter, weeklySchedules);

            var fileName = $"Schedule for Driver {filter.FromDate.ToString(DateFormat)} to {filter.ToDate.ToString(DateFormat)}.xlsx";
            return File(bytes, MimeType.XLSX, fileName);
        }

        private bool CheckPermission(DateTime fromDate)
        {
            var firstDayOfWeek = DateTimeOffset.Now.Date.FirstDayOfWeek();

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
                const int NextWeek = 7;
                isHost = fromDate.Date >= firstDayOfWeek.AddDays(NextWeek);
            }
            else
            {
                isHost = false;
            }

            return isHost;
        }
    }
}