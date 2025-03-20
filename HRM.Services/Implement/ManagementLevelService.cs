using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.ManagementLevel;
using HRM.Models;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class ManagementLevelService(IBaseService baseService) : IManagementLevelService
{
    public async Task<IList<ManagementLevel>> GetManagementLevel()
    {
        var criteria = new GetManagementLevelCriteria();

        return (await baseService.GetDataAsync<ManagementLevel>(criteria)).ToList();
    }

    public async Task<CommandResult> Insert(InsertManagementLevelCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> Update(UpdateManagementLevelCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> Delete(short id, int actorId)
    {
        var criteria = new DeleteManagementLevelCriteria
        {
            ManagementLevelId = id,
            ModifiedBy = actorId
        };

        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}