namespace HRM.Controllers;

using System;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Filters;
using HRM.Core.Models;
using HRM.Models;
using HRM.Services;
using Microsoft.AspNetCore.Mvc;

public class HolidayController(
    IUserControlSession userControlSession,
    IHolidayService holidayService) : BaseController(userControlSession)
{
    [HttpGet]
    [HrmPermission(Policy.Read, FunctionKey.Holiday)]
    public async Task<IActionResult> Index(int? year)
    {
        var yearToCheck = year ?? DateTimeOffset.Now.DateTime.ToClientTimeZone().Year;
        ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.Holiday, Policy.Host);
        ViewBag.Year = yearToCheck;

        var holidays = await holidayService.GetByYearAsync(yearToCheck);

        return View(holidays);
    }

    [HrmPermission(Policy.Host, FunctionKey.Holiday)]
    public async Task<IActionResult> Edit(int id)
    {
        var holiday = await holidayService.GetByIdAsync(id);

        return View(holiday);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.Holiday)]
    public async Task<IActionResult> Edit(Holiday model)
    {
        if (ModelState.IsValid)
        {
            var commandResult = await holidayService.EditAsync(EmployeeSession.EmpId, model);
            ViewBag.Updated = commandResult.IsSuccessful;

            if (!commandResult.IsSuccessful)
            {
                ModelState.AddModelError(string.Empty, commandResult.Message);
            }
        }

        return View(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.Holiday)]
    public ActionResult Create()
    {
        var currentDateTime = DateTimeOffset.Now.DateTime.ToClientTimeZone();
        var holiday = new Holiday
        {
            TypeId = (byte)HolidayType.Regular,
            FromDate = currentDateTime,
            ToDate = currentDateTime
        };

        return View(holiday);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.Holiday)]
    public async Task<IActionResult> Create(Holiday model)
    {
        if (ModelState.IsValid)
        {
            var commandResult = await holidayService.CreateAsync(EmployeeSession.EmpId, model);
            ViewBag.Updated = commandResult.IsSuccessful;

            if (!commandResult.IsSuccessful)
            {
                ModelState.AddModelError(string.Empty, commandResult.Message);
            }
        }

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.Holiday)]
    public async Task<IActionResult> Delete(int id)
    {
        var commandResult = await holidayService.DeleteAsync(EmployeeSession.EmpId, id);

        return Json(commandResult);
    }
}