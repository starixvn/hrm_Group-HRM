namespace HRM.Controllers;

using System;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Filters;
using Criteria.Calendar;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using Microsoft.AspNetCore.Mvc;
using Services.Implement;
using Services.Interfaces;
using ViewModels;
using ViewModels.Calendar;

public class CalendarController(
    IUserControlSession session,
    IWorkingCalendarService workingCalendarService,
    IShiftService shiftService,
    ICommonDataService commonDataService,
    ITeamService teamService) : BaseController(session)
{
    public async Task<IActionResult> GetShift(string query, bool isShowOff = false)
    {
        var trimmedQuery = query?.Trim() ?? string.Empty;
        var shifts = await shiftService.GetByQueryAsync(trimmedQuery, isShowOff);

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
            EmpId = session.UserId,
            FromDate = date.FirstDayOfMonth(),
            ToDate = date.AddMonths(1).AddDays(-1).LastDayOfMonth(),
            CompanyId = CurrentCompanyId
        };
        var workingSchedule = await workingCalendarService.GetWorkingScheduleAsync(criteria);

        return Json(workingSchedule);
    }

    [HrmPermission(Policy.Read, FunctionKey.WorkingCalendars)]
    public async Task<IActionResult> WorkingCalendars()
    {
        var currentDateTime = DateTimeOffset.Now.DateTime.ToClientTimeZone();
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
            IsLocal = model.IsLocal,
            CompanyId = CurrentCompanyId
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
        var data = await commonDataService.GetLookupDataByCodeAsync(Constants.JobTitle, CurrentCompanyId);
        var jobTitles = data.Select(x => new MultiSelectViewModel
        {
            Id = x.Id,
            Text = x.Name,
            IsLeaf = true
        }).ToList();

        return Json(jobTitles);
    }
}