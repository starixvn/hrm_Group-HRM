using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Models.Shifts;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRM.Services.Interfaces;

public interface IShiftService
{
    Task<IList<Shift>> GetAllAsync(int statusId = 1, bool isShowOff = false);

    Task<IList<Shift>> GetByQueryAsync(string query, bool isShowOff);

    Task<IList<SelectListItem>> GetSelectListAsync(int statusId = 1, bool isShowOff = false);

    Task<Shift> GetByDateAsync(int employeeId, DateTime date);

    Task<Shift> GetByIdAsync(int shiftId);

    Task<CommandResult> CreateAsync(int actorId, CreateShiftModel model);

    Task<CommandResult> EditAsync(int actorId, EditShiftModel model);

    Task<CommandResult> DeleteAsync(int shiftId);
}