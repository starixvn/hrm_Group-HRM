namespace HRM.Controllers
{
    using System;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Filters;
    using HRM.Core.Models;
    using HRM.Models;
    using HRM.Services;
    using HRM.ViewModels.Shifts;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using HRM.Services;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    public class ShiftController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IShiftService shiftService;

        public ShiftController(
            IUserControlSession userControlSession,
            IShiftService shiftService) : base(userControlSession)
        {
            this.userControlSession = userControlSession;
            this.shiftService = shiftService;
        }

        //[HrmPermission(Policy.Read, FunctionKey.ShiftManagement)]
        //public ActionResult Index(int? statusId = (int)ShiftStatus.Active)
        //{
        //    var model = new ShiftByStatusViewModel
        //    {
        //        StatusId = statusId.Value,
        //        Shifts = shiftService.GetAll(statusId.Value)
        //    };
        //    ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.ShiftManagement, Policy.Host);

        //    return View(model);
        //}

        //[HRMPermission(Policy.Host, FunctionKey.ShiftManagement)]
        //public ActionResult Create()
        //{
        //    var model = new CreateShiftModel();
        //    ViewBag.Successed = false;
        //    return View(model);
        //}

        //[ValidateInput(false)]
        //[HttpPost]
        //[HRMPermission(Policy.Host, FunctionKey.ShiftManagement)]
        //public ActionResult Create(CreateShiftModel model)
        //{
        //    if (model != null && ModelState.IsValid)
        //    {
        //        CommandResult commandResult = shiftService.Create(EmployeeSession.EmpId, model);
        //        if (commandResult.IsSuccessful)
        //        {
        //            return RedirectToAction("Index", "Shift");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, commandResult.Message);
        //        }
        //    }

        //    return View(model);
        //}

        //[HRMPermission(Policy.Host, FunctionKey.ShiftManagement)]
        //public ActionResult Edit(int id)
        //{
        //    const byte HourIndex = 0;
        //    const byte MinutesIndex = 1;
        //    Shift shift = shiftService.GetById(id);
        //    EditShiftModel model = null;

        //    if (shift != null)
        //    {
        //        model = new EditShiftModel
        //        {
        //            ShiftId = shift.Id,
        //            Name = shift.Name,
        //            HourTimeIn = GetTime(shift.BeginCheckInTime, HourIndex),
        //            MinuteTimeIn = GetTime(shift.BeginCheckInTime, MinutesIndex),
        //            HourTimeOut = GetTime(shift.BeginCheckOutTime, HourIndex),
        //            MinuteTimeOut = GetTime(shift.BeginCheckOutTime, MinutesIndex),
        //            IsInAfterDay = shift.IsInAfterDay,
        //            IsOutAfterDay = shift.IsOutAfterDay,
        //            IsDisabled = shift.IsDisabled,
        //            IsUsed = shift.IsUsed
        //        };
        //    }

        //    ViewBag.Successed = false;
        //    ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.ShiftManagement, Policy.Host);

        //    return View(model);
        //}

        //[ValidateInput(false)]
        //[HttpPost]
        //[HRMPermission(Policy.Host, FunctionKey.ShiftManagement)]
        //public ActionResult Edit(EditShiftModel model)
        //{
        //    if (model != null && ModelState.IsValid)
        //    {
        //        CommandResult commandResult = shiftService.Edit(EmployeeSession.EmpId, model);
        //        if (commandResult.IsSuccessful)
        //        {
        //            return RedirectToAction("Index", "Shift");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, commandResult.Message);
        //        }
        //    }

        //    ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.ShiftManagement, Policy.Host);

        //    return View(model);
        //}

        //[HttpPost]
        //[HRMPermission(Policy.Host, FunctionKey.ShiftManagement)]
        //public JsonResult Delete(int shiftId)
        //{
        //    CommandResult commandResult = shiftService.Delete(shiftId);
        //    return Json(commandResult);
        //}

        public async Task<IActionResult> GetShiftByDate(int employeeId, DateTime date)
        {
            var shift = shiftService.GetByDate(employeeId, date);
            return Json(shift);
        }

        private static int GetTime(string time, int index)
        {
            var value = 0;
            if (!string.IsNullOrWhiteSpace(time))
            {
                var timeArr = time.Split(':');
                const byte TotalItems = 2;
                if (timeArr.Length == TotalItems)
                {
                    value = int.Parse(timeArr[index]);
                }
            }

            return value;
        }
    }
}