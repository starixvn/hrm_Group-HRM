namespace HRM.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Criteria.Unit;
using HRM.Models;

public interface IUnitService
{
    Task<Unit> GetByIdAsync(int id);

    Task<IList<Unit>> GetUnitAsync(int companyId, bool? isActive = true);

    Task<CommandResult> CreateAsync(InsertUnitCriteria criteria);

    Task<CommandResult> EditAsync(UpdateUnitCriteria criteria);

    Task<CommandResult> DeleteAsync(DeleteUnitCriteria criteria);
}