using System.Threading.Tasks;

namespace HRM.Services.Interfaces;

using HRM.Core.Models;
using Criteria.JobLevel;
using Models;
using System.Collections.Generic;

public interface IJobLevelService
{
    Task<IList<JobLevel>> GetJobLevel();

    Task<CommandResult> Insert(InsertJobLevelCriteria criteria);

    Task<CommandResult> Update(UpdateJobLevelCriteria criteria);

    Task<CommandResult> Delete(int id, int actorId);
}