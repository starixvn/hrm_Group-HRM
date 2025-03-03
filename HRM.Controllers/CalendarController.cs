namespace HRM.Controllers;

using System;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using Core;
using Core.Constants;
using Core.Filters;
using Criteria.Calendar;
using Services;
using ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Implement;

public class CalendarController(
    IUserControlSession userControlSession,
    IWorkingCalendarService workingCalendarService,
    IShiftService shiftService,
    ICommonDataService commonDataService,
    ITeamService teamService) : BaseController(userControlSession)
{
    public async Task<IActionResult> GetShift(string query, bool isShowOff = false)
    {
        var shifts = await shiftService.GetByQueryAsync(query.Trim(), isShowOff);

        return Json(shifts);
    }

    public ActionResult MyWorkingSchedule()
    {
        return View();
    }

    public async Task<IActionResult> GetWorkingSchedule(DateTime date)
    {
        var criteria = new GetEmployeeCalendarByMonthCriteria
        {
            EmpId = EmployeeSession.EmpId,
            FromDate = date.FirstDayOfMonth(),
            ToDate = date.AddMonths(1).AddDays(-1).LastDayOfMonth()
        };
        var workingSchedule = await workingCalendarService.GetWorkingScheduleAsync(criteria);

        return Json(workingSchedule);
    }

    [HrmPermission(Policy.Read, FunctionKey.WorkingCalendars)]
    public async Task<IActionResult> WorkingCalendars()
    {
        var currentDateTime = DateTimeOffset.Now.DateTime;
        var teamIds = await teamService.GetAllAsync();
        var model = new WorkingCalendarFilterViewModel
        {
            FromDate = currentDateTime.FirstDayOfWeek(),
            ToDate = currentDateTime.LastDayOfWeek(),
            TeamIds = string.Join(",", teamIds.Select(x => x.Id))
        };

        return View(model);
    }

    [HrmPermission(Policy.Read, FunctionKey.WorkingCalendars)]
    public async Task<IActionResult> WorkingCalendarDetails(WorkingCalendarFilterViewModel model)
    {
        var criteria = new GetWorkingCalendarWeeklyCriteria
        {
            EmpId = model.EmployeeId,
            Fromtime = model.FromDate,
            Totime = model.ToDate,
            TeamIds = model.TeamIds,
            JobTitleId = model.JobTitleIds,
            IsLocal = model.IsLocal
        };
        var workingCalendars = await workingCalendarService.GetWorkingCalendarsAsync(criteria);

        var workingCalendarDetails = new WorkingCalendarViewModel
        {
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            WorkingCalendars = workingCalendars
        };

        return PartialView("_WorkingCalendarDetails", workingCalendarDetails);
    }

    public async Task<IActionResult> GetJobTitle()
    {
        var data = await commonDataService.GetLookupDataByCodeAsync(Constants.JobTitle);
        var jobTitles = data.Select(x => new MultiSelectViewModel
        {
            Id = x.Id,
            Text = x.Name,
            IsLeaf = true
        }).ToList();

        return Json(jobTitles);
    }
}