using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Unit;
using HRM.Models.Unit;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class UnitService(
    IBaseService baseService) : IUnitService
{
    public async Task<Unit> GetByIdAsync(int id)
    {
        var criteria = new GetUnitByIdCriteria
        {   
            UnitId = id
        };

        return await baseService.GetDataEntityAsync<Unit>(criteria);
    }

    public async Task<int> GetNextGroupUnitIdAsync()
    {
        var criteria = new GetNextGroupUnitIdCriteria();
        return await baseService.GetDataEntityAsync<int>(criteria);
    }

    public async Task<IEnumerable<Unit>> GetUnitAsync(bool? isActive = true)
    {
        var criteria = new GetUnitCriteria
        {
            IsActive = isActive
        };

        return await baseService.GetDataAsync<Unit>(criteria);
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