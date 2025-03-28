namespace HRM.Controllers;

using System;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Enums;
using Core.Filters;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Interfaces;

public class HolidayController(
    IUserControlSession session,
    IHolidayService holidayService) : BaseController(session)
{
    [HttpGet]
    [HrmPermission(Policy.Read, FunctionKey.Holiday)]
    public async Task<IActionResult> Index(int? year)
    {
        var yearToCheck = year ?? DateTimeOffset.Now.DateTime.ToClientTimeZone().Year;
        ViewBag.HasPermission = session.HasPermissionOnFunction(FunctionKey.Holiday, Policy.Host);
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
            var commandResult = await holidayService.EditAsync(session.UserId, model);
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
            var commandResult = await holidayService.CreateAsync(session.UserId, model);
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
        var commandResult = await holidayService.DeleteAsync(session.UserId, id);

        return Json(commandResult);
    }
}