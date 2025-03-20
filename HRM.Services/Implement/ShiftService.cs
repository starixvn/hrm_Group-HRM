using Microsoft.AspNetCore.Mvc;

namespace HRM.Services.Implement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Constants;
using Core.Enums;
using Criteria.Shift;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Models;
using HRM.Core.Services;
using Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Shifts;

public class ShiftService(
    IBaseService baseService,
    IUserControlSession userControlSession) : IShiftService
{
    public async Task<IList<Shift>> GetAllAsync(int statusId = 1, bool isShowOff = false)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetAllShiftCriteria
        {
            CompanyId = companyId
        };
        var shifts = await baseService.GetDataAsync<Shift>(criteria);

        shifts = statusId switch
        {
            (int)ShiftStatus.Active => shifts.Where(t => !t.IsDisabled).ToList(),
            (int)ShiftStatus.Disabled => shifts.Where(t => t.IsDisabled).ToList(),
            _ => shifts
        };

        if (isShowOff)
        {
            shifts = [new Shift { Code = "Off", Id = -1 }, .. shifts];
        }

        return shifts.ToList();
    }

    public async Task<IList<Shift>> GetByQueryAsync(string query, bool isShowOff)
    {
        var shifts = await GetAllAsync(isShowOff: isShowOff);
        IList<Shift> queriedShifts = null;

        if (shifts != null && shifts.Any())
        {
            queriedShifts = shifts.Where(t => t.Code.ToUpperInvariant().StartsWith(query.ToUpperInvariant(), StringComparison.CurrentCulture)).ToList();
        }

        return queriedShifts ?? new List<Shift>();
    }

    public async Task<IList<SelectListItem>> GetSelectListAsync(int statusId = 1, bool isShowOff = false)
    {
        var list = new List<SelectListItem>();
        var shifts = await GetAllAsync(statusId, isShowOff);

        if (shifts.Any())
        {
            list = shifts.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
        }

        return list;
    }

    public async Task<Shift> GetByDateAsync(int employeeId, DateTime date)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);

        var criteria = new GetEmployeeShiftByDateCriteria
        {
            EmployeeId = employeeId,
            Date = date,
            CompanyId = companyId
        };
        return await baseService.GetDataEntityAsync<Shift>(criteria);
    }

    public async Task<Shift> GetByIdAsync(int shiftId)
    {
        var criteria = new GetShiftByIdCriteria { ShiftId = shiftId };
        return await baseService.GetDataEntityAsync<Shift>(criteria);
    }

    public async Task<CommandResult> CreateAsync(int actorId, CreateShiftModel model)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new CreateShiftCriteria
        {
            Name = model.Name,
            CheckInTime = model.TimeIn,
            CheckOutTime = model.TimeOut,
            IsInAfterDay = model.IsInAfterDay,
            IsOutAfterDay = model.IsOutAfterDay,
            ActorId = actorId,
            CompanyId = companyId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> EditAsync(int actorId, EditShiftModel model)
    {
        var criteria = new EditShiftCriteria
        {
            ShiftId = model.ShiftId,
            Name = model.Name,
            CheckInTime = model.TimeIn,
            CheckOutTime = model.TimeOut,
            IsInAfterDay = model.IsInAfterDay,
            IsOutAfterDay = model.IsOutAfterDay,
            IsDisabled = model.IsDisabled,
            ActorId = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> DeleteAsync(int shiftId)
    {
        var criteria = new DeleteShiftCriteria { ShiftId = shiftId };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}