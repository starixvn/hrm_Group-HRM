namespace HRM.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using Criteria.Unit;
using Models;

public interface IUnitService
{
    Task<Unit> GetByIdAsync(int id);

    Task<IList<Unit>> GetUnitAsync(bool? isActive = true);

    Task<CommandResult> CreateAsync(InsertUnitCriteria criteria);

    Task<CommandResult> EditAsync(UpdateUnitCriteria criteria);

    Task<CommandResult> DeleteAsync(DeleteUnitCriteria criteria);
}