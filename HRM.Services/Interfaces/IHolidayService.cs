namespace HRM.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using Models;

public interface IHolidayService
{
    Task<Holiday> GetByIdAsync(int holidayId);

    Task<IEnumerable<Holiday>> GetByYearAsync(int year);

    Task<CommandResult> CreateAsync(int actorId, Holiday model);

    Task<CommandResult> EditAsync(int actorId, Holiday model);

    Task<CommandResult> DeleteAsync(int actorId, int id);
}