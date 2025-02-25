using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;

    public class ScheduleArrangeController : BaseController
    {
        private const string DateFormat = "dd-MMM-yyyy";
        private readonly IUserControlSession userControlSession;
        private readonly IRoleService roleService;
        private readonly IScheduleArrangeService scheduleArrangeService;
        private readonly ISyncWorkingShiftService syncWorkingShiftService;
        private readonly IExportExcelScheduleService exportExcelService;

        public ScheduleArrangeController(
            IUserControlSession userControlSession,
            IRoleService roleService,
            IScheduleArrangeService scheduleArrangeService,
            ISyncWorkingShiftService syncWorkingShiftService,
            IExportExcelScheduleService exportExcelService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.roleService = roleService;
            this.scheduleArrangeService = scheduleArrangeService;
            this.syncWorkingShiftService = syncWorkingShiftService;
            this.exportExcelService = exportExcelService;
        }

        [HttpGet]
        [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult Index()
        {
            ViewBag.HasPermissionToViewHoliday = userControlSession.HasPermissionOnFunction(FunctionKey.Holiday, Policy.Read);
            ViewBag.HasPermissionToViewShift = userControlSession.HasPermissionOnFunction(FunctionKey.ShiftManagement, Policy.Read);
            ViewBag.EnableSyncWorkingShift = ConfigSettings.EnableManualSyncWorkingShift;
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
        public CustomJsonResult GetEmployees(GetEmployeeFilter filter)
        {
            filter.ActorId = EmployeeSession.EmpId;
            IEnumerable<EmployeeSuggestion> employees = scheduleArrangeService.GetEmployees(filter);

            return new CustomJsonResult { Data = employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [AllowAnonymous]
        public CustomJsonResult GetWeeklySchedule(WeeklyScheduleFilter filter)
        {
            filter.ActorId = EmployeeSession.EmpId;
            IEnumerable<WeeklySchedule> weeklySchedules = scheduleArrangeService.GetWeeklySchedule(filter);
            IEnumerable<WeeklyScheduleViewModel> schedules = weeklySchedules.GroupBy(x => new { x.Team.Id, x.Team.Code, x.Team.Name })
                .Select(x => new WeeklyScheduleViewModel
                {
                    TeamId = x.Key.Id,
                    Name = x.Key.Name,
                    Code = x.Key.Code,
                    Members = x
                });

            bool isHost = CheckPermission(filter.FromDate);
            var data = new { isHost, schedules };

            return new CustomJsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditWeekly(
            DateTime fromDate,
            DateTime toDate,
            IEnumerable<EditDailySchedule> weeklySchedules,
            IEnumerable<EditDefaultShift> defaultShifts)
        {
            CommandResult commandResult = scheduleArrangeService.EditWeekly(
                EmployeeSession.EmpId,
                fromDate,
                toDate,
                weeklySchedules,
                defaultShifts);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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
            CommandResult commandResult = await syncWorkingShiftService.SyncWorkingShiftAsync(command);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditRemark(EditRemark model)
        {
            CommandResult commandResult = scheduleArrangeService.EditRemark(EmployeeSession.EmpId, model);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditDisplayOrder(IEnumerable<EditDisplayOrder> displayOrders)
        {
            CommandResult commandResult = scheduleArrangeService.EditDisplayOrder(EmployeeSession.EmpId, displayOrders);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditTeamOrder(IEnumerable<EditTeamOrder> displayOrders)
        {
            CommandResult commandResult = scheduleArrangeService.EditTeamOrder(EmployeeSession.EmpId, displayOrders);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditWeeklyDefaultShift(WeeklyDefaultShift weeklyDefaultShift, bool isUsedWeeklyDefaultShift)
        {
            CommandResult commandResult = scheduleArrangeService.EditWeeklyDefaultShift(EmployeeSession.EmpId, weeklyDefaultShift, isUsedWeeklyDefaultShift);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult ExportExcel(WeeklyScheduleFilter filter)
        {
            filter.ActorId = EmployeeSession.EmpId;
            IEnumerable<WeeklySchedule> weeklySchedules = scheduleArrangeService.GetWeeklySchedule(filter);
            byte[] bytes = exportExcelService.Export(filter, weeklySchedules);

            var fileName = $"Working shift {filter.FromDate.ToString(DateFormat)} to {filter.ToDate.ToString(DateFormat)}.xlsx";
            return File(bytes, MimeType.XLSX, fileName);
        }

        [HttpGet]
        [HrmPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult ExportExcelDriverPickup(WeeklyScheduleFilter filter)
        {
            Actor actor = new Actor
            {
                ActorId = EmployeeSession.EmpId,
                Nickname = EmployeeSession.Nickname,
                FullName = EmployeeSession.FullName
            };
            filter.ActorId = EmployeeSession.EmpId;
            IEnumerable<WeeklySchedule> weeklySchedules = scheduleArrangeService.GetWeeklySchedule(filter);
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