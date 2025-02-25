namespace HRM.Services.Interfaces
{
    using HRM.Core.Models;
    using HRM.Criteria.JobLevel;
    using HRM.Models;
    using System.Collections.Generic;

    public interface IJobLevelService
    {
        IList<JobLevel> GetJobLevel();

        CommandResult Insert(InsertJobLevelCriteria criteria);

        CommandResult Update(UpdateJobLevelCriteria criteria);

        CommandResult Delete(int id, int actorId);
    }
}
