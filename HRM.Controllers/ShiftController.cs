using HRM.Models.Shifts;
using HRM.Services.Interfaces;

namespace HRM.Controllers;

using System;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using Core.Constants;
using Core.Enums;
using Core.Filters;
using ViewModels.Shifts;
using Microsoft.AspNetCore.Mvc;

public class ShiftController(
    IUserControlSession userControlSession,
    IShiftService shiftService) : BaseController(userControlSession)
{
    [HrmPermission(Policy.Read, FunctionKey.ShiftManagement)]
    public async Task<IActionResult> Index(int? statusId = (int)ShiftStatus.Active)
    {
        var model = new ShiftByStatusViewModel
        {
            StatusId = statusId.Value,
            Shifts = await shiftService.GetAllAsync(statusId.Value)
        };
        ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.ShiftManagement, Policy.Host);

        return View(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.ShiftManagement)]
    public ActionResult Create()
    {
        var model = new CreateShiftModel();
        ViewBag.Successed = false;
        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ShiftManagement)]
    public async Task<IActionResult> Create(CreateShiftModel model)
    {
        if (model != null && ModelState.IsValid)
        {
            var commandResult = await shiftService.CreateAsync(EmployeeSession.EmpId, model);
            if (commandResult.IsSuccessful)
            {
                return RedirectToAction("Index", "Shift");
            }
            else
            {
                ModelState.AddModelError(string.Empty, commandResult.Message);
            }
        }

        return View(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.ShiftManagement)]
    public async Task<IActionResult> Edit(int id)
    {
        const byte HourIndex = 0;
        const byte MinutesIndex = 1;
        var shift = await shiftService.GetByIdAsync(id);
        EditShiftModel model = null;

        if (shift != null)
        {
            model = new EditShiftModel
            {
                ShiftId = shift.Id,
                Name = shift.Name,
                HourTimeIn = GetTime(shift.BeginCheckInTime, HourIndex),
                MinuteTimeIn = GetTime(shift.BeginCheckInTime, MinutesIndex),
                HourTimeOut = GetTime(shift.BeginCheckOutTime, HourIndex),
                MinuteTimeOut = GetTime(shift.BeginCheckOutTime, MinutesIndex),
                IsInAfterDay = shift.IsInAfterDay,
                IsOutAfterDay = shift.IsOutAfterDay,
                IsDisabled = shift.IsDisabled,
                IsUsed = shift.IsUsed
            };
        }

        ViewBag.Successed = false;
        ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.ShiftManagement, Policy.Host);

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ShiftManagement)]
    public async Task<IActionResult> Edit(EditShiftModel model)
    {
        if (model != null && ModelState.IsValid)
        {
            var commandResult = await shiftService.EditAsync(EmployeeSession.EmpId, model);
            if (commandResult.IsSuccessful)
            {
                return RedirectToAction("Index", "Shift");
            }
            else
            {
                ModelState.AddModelError(string.Empty, commandResult.Message);
            }
        }

        ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.ShiftManagement, Policy.Host);

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.ShiftManagement)]
    public async Task<IActionResult> Delete(int shiftId)
    {
        var commandResult = await shiftService.DeleteAsync(shiftId);
        return Json(commandResult);
    }

    public async Task<IActionResult> GetShiftByDate(int employeeId, DateTime date)
    {
        var shift = await shiftService.GetByDateAsync(employeeId, date);
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