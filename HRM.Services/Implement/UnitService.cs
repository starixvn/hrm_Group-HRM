namespace HRM.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Unit;
using HRM.Models;

public class UnitService(IBaseService baseService) : IUnitService
{
    public async Task<Unit> GetByIdAsync(int id)
    {
        var criteria = new GetUnitByIdCriteria { UnitId = id };

        return await baseService.GetDataEntityAsync<Unit>(criteria);
    }

    public async Task<IList<Unit>> GetUnitAsync(bool? isActive = true)
    {
        var criteria = new GetUnitsCriteria { IsActive = isActive };

        var data = await baseService.GetDataAsync<Unit>(criteria);
        return data.ToList();
    }

    public async Task<CommandResult> CreateAsync(InsertUnitCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> EditAsync(UpdateUnitCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> DeleteAsync(DeleteUnitCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}