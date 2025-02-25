namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.JobLevel;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface IJobLevelService
    {
        IList<JobLevel> GetJobLevel();

        CommandResult Insert(InsertJobLevelCriteria criteria);

        CommandResult Update(UpdateJobLevelCriteria criteria);

        CommandResult Delete(int id, int actorId);
    }
}
