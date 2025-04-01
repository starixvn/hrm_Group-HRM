using System.Threading.Tasks;

namespace HRM.Services.Implement;

using HRM.Core.Models;
using HRM.Core.Services;
using Criteria.JobLevel;
using Models;
using Interfaces;
using System.Collections.Generic;
using System.Linq;

public class JobLevelService(IBaseService baseService) : IJobLevelService
{
    public async Task<IList<JobLevel>> GetJobLevel()
    {
        var criteria = new GetJobLevelCriteria();

        return (await baseService.GetDataAsync<JobLevel>(criteria)).ToList();
    }

    public async Task<CommandResult> Insert(InsertJobLevelCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> Update(UpdateJobLevelCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> Delete(int id, int actorId)
    {
        var criteria = new DeleteJobLevelCriteria
        {
            JobLevelId = id,
            ModifiedBy = actorId
        };

        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}