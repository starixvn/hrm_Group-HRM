namespace HRM.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Filters;
using Core.XMLExtensions;
using Criteria.Discipline;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Enums;
using HRM.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Discipline;
using Resources;
using Services.Interfaces;
using ViewModels.Discipline;

public class DisciplineController(
    IUserControlSession session,
    IRoleService roleService,
    IDisciplineService disciplineService,
    IBaseService baseService) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.Discipline)]
    public async Task<IActionResult> DisciplineSummary(DisciplineSummaryViewModel model)
    {
        if (model != null)
        {
            model.FromDate = GetFromDate();
            model.ToDate = GetToDate();
            model.DisciplineTypes = await GetDisciplineType(true);
            model.DateRange = GetDateRangeByQuarter();
        }

        ViewBag.RoleId = (short)RoleSystem.DisciplineManager;
        ViewBag.CompanyId = CurrentCompanyId;
        ViewBag.IsHost = session.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);
        ViewBag.IsHostOtherDiscipline = session.HasPermissionOnFunction(FunctionKey.OtherDiscipline, Policy.Host);
        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.Discipline)]
    public async Task<IActionResult> GetDisciplineSummaryGrid(DisciplineSummaryViewModel model, int? companyId = null)
    {
        var roles = await roleService.GetByUserIdAsync(EmployeeSession.EmpId);
        var criteria = new GetDisciplineSummaryCriteria
        {
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamId,
            IsLocal = model.IsLocal,
            EmployeeId = model.EmployeeId,
            ReasonId = model.ReasonId,
            ApproverId = EmployeeSession.EmpId,
            IsShowAll = model.IsShowAll,
            CompanyId = (short)(companyId ?? CurrentCompanyId),
            RoleIds = string.Join(",", roles.Select(x => x.RoleId))
        };

        var disciplines = await disciplineService.GetDisciplineSummaryAsync(criteria);

        ViewBag.IsHost = session.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);

        return PartialView("_DisciplineSummaryGrid", disciplines);
    }

    [HrmPermission(Policy.Read, FunctionKey.Discipline)]
    public async Task<IActionResult> DisciplineDetail(DisciplineSummaryViewModel model, short? companyId = null)
    {
        if (model != null)
        {
            if (model.FromDate == default)
            {
                model.FromDate = GetFromDate();
                model.ToDate = GetToDate();
            }

            if (model.ReasonId < 0)
            {
                model.ReasonId = null;
            }

            model.DisciplineTypes = await GetDisciplineType(false);
            model.DateRange = GetDateRangeByQuarter();
        }

        ViewBag.RoleId = (short)RoleSystem.DisciplineManager;
        ViewBag.CompanyId = companyId ?? CurrentCompanyId;
        ViewBag.IsHost = session.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);

        return View(model);
    }

    [HrmPermission(Policy.Read, FunctionKey.Discipline)]
    public async Task<IActionResult> GetDisciplineDetailGrid(DisciplineSummaryViewModel model, short? companyId = null)
    {
        var roles = await roleService.GetByUserIdAsync(session.UserId);
        var criteria = new GetDisciplineDetailCriteria
        {
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamId,
            IsLocal = model.IsLocal,
            EmployeeId = model.EmployeeId,
            ReasonId = model.ReasonId,
            HasIgnored = model.HasIgnored,
            ApproverId = EmployeeSession.EmpId,
            CompanyId = companyId ?? CurrentCompanyId,
            RoleIds = string.Join(",", roles.Select(x => x.RoleId))
        };

        var disciplines = await disciplineService.GetDisciplineDetailAsync(criteria);
        ViewBag.HasPermissionToEdit = session.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);
        ViewBag.HasPermissionToUpdateOutOfControlReason = session.HasPermissionOnFunction(FunctionKey.UpdateOutOfControlReason, Policy.Host);

        return PartialView("_DisciplineDetailGrid", disciplines);
    }

    [HrmPermission(Policy.Read, FunctionKey.Discipline)]
    public async Task<IActionResult> ViewOtherDiscipline(int employeeId, DateTime fromDate, DateTime toDate, short? companyId = null)
    {
        var criteria = new GetOtherDisciplineOfEmployeeCriteria
        {
            EmployeeId = employeeId,
            FromDate = fromDate,
            ToDate = toDate
        };

        var otherDisciplines = await disciplineService.GetOtherDisciplineAsync(criteria);
        var members = await disciplineService.GetMemberToAddOtherDisciplineAsync(EmployeeSession.EmpId, null, companyId);
        ViewBag.IsHost = members.Any(x => x.EmployeeId == employeeId)
            && session.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);

        return PartialView("_ViewOtherDiscipline", otherDisciplines);
    }

    [HrmPermission(Policy.Host, FunctionKey.OtherDiscipline)]
    public async Task<IActionResult> PartialAddOtherDiscipline(string employee, int? employeeId = null)
    {
        var criteria = new GetOtherDisciplineTypeCriteria();
        var otherDisciplineTypes = await disciplineService.GetOtherDisciplineTypeAsync(criteria);

        var model = new AddOtherDisciplineViewModel
        {
            EmployeeId = employeeId ?? -1,
            Employee = employee,
            WorkDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date,
            OtherDisciplineTypes = otherDisciplineTypes
        };

        return PartialView("_AddOtherDiscipline", model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.OtherDiscipline)]
    public async Task<IActionResult> AddOtherDiscipline(AddOtherDisciplineViewModel model)
    {
        var criteria = new InsertOtherDisciplineCriteria
        {
            EmployeeId = model.EmployeeId,
            WorkDate = model.WorkDate,
            DisciplineTypeId = model.DisciplineTypeId,
            Score = model.MinusScore,
            Comment = model.Comment,
            ActorId = EmployeeSession.EmpId
        };
        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return Json(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.OtherDiscipline)]
    public async Task<string> DeleteOtherDiscipline(DeleteOtherDisciplineCriteria criteria)
    {
        criteria.ActorId = EmployeeSession.EmpId;
        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return !commandResult.IsSuccessful ? commandResult.Message : string.Empty;
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.Discipline)]
    public async Task<IActionResult> EditDisciplineIgnore(
        IEnumerable<IgnoredDisciplineModel> ignoreDetails,
        IEnumerable<IgnoredDisciplineDetailModel> disciplineIgnores)
    {
        var criteria = new UpdateIgnoredDisciplineCriteria
        {
            IgnoreDetailXml = ignoreDetails.ToXML().ToString(),
            DisciplineIgnoreXml = disciplineIgnores.ToXML().ToString(),
            HasUpdateOutOfControlReason = session.HasPermissionOnFunction(FunctionKey.UpdateOutOfControlReason, Policy.Host),
            ActorId = EmployeeSession.EmpId
        };
        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return Json(commandResult);
    }

    public async Task<IActionResult> GetMemberToAddOtherDiscipline(string keyword, short? companyId = null)
    {
        var employees = await disciplineService.GetMemberToAddOtherDisciplineAsync(EmployeeSession.EmpId, keyword, companyId);

        return Json(employees);
    }

    private static IList<SelectListItem> GetDateRangeByQuarter()
    {
        var dateRange = new List<SelectListItem> { new() { Text = Labels.DateRange, Value = "0" } };
        var currentDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        const byte May = 5;
        const byte June = 6;
        const byte December = 12;
        for (var i = 0; i < June; i++)
        {
            var currentMonth = currentDate.Month;
            var currentYear = currentDate.Year;
            switch (currentMonth)
            {
                case December:
                    dateRange.Add(new SelectListItem
                    {
                        Text = $"1H - {currentYear + 1}",
                        Value = $"1{currentYear + 1}"
                    });
                    break;

                case >= 1 and <= May:
                    dateRange.Add(new SelectListItem
                    {
                        Text = $"1H - {currentYear}",
                        Value = $"1{currentYear}"
                    });
                    break;

                default:
                    dateRange.Add(new SelectListItem
                    {
                        Text = $"2H - {currentYear}",
                        Value = $"2{currentYear}"
                    });
                    break;
            }

            currentDate = currentDate.AddMonths(-June);
        }

        return dateRange;
    }

    private static DateTime GetFromDate()
    {
        const byte May = 5;
        const byte June = 6;
        const byte December = 12;
        var (year, month, _) = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        var fromDate = month switch
        {
            December => new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero),
            >= 1 and <= May => new DateTimeOffset(year - 1, December, 1, 0, 0, 0, TimeSpan.Zero),
            _ => new DateTimeOffset(year, June, 1, 0, 0, 0, TimeSpan.Zero)
        };

        return fromDate.DateTime;
    }

    private static DateTime GetToDate()
    {
        const byte May = 5;
        const byte December = 12;
        var (year, month, _) = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        var toDate = month switch
        {
            December => new DateTimeOffset(year + 1, May, DateTime.DaysInMonth(year + 1, May), 0, 0, 0,
                TimeSpan.Zero),
            >= 1 and <= May => new DateTimeOffset(year, May, DateTime.DaysInMonth(year, May), 0, 0, 0,
                TimeSpan.Zero),
            _ => new DateTimeOffset(year, December - 1, DateTime.DaysInMonth(year, December - 1), 0, 0, 0,
                TimeSpan.Zero)
        };

        return toDate.DateTime;
    }

    private async Task<IList<DisciplineTypeModel>> GetDisciplineType(bool includeOtherType)
    {
        var criteria = new GetDisciplineTypeCriteria
        {
            IncludeOtherType = includeOtherType
        };
        var disciplineTypes = await disciplineService.GetDisciplineTypeAsync(criteria);

        return disciplineTypes;
    }
}