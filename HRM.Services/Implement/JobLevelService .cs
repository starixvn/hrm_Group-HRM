namespace HRM.Services.Implement
{
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.JobLevel;
    using HRM.Models;
    using HRM.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class JobLevelService : IJobLevelService
    {
        private readonly IBaseService baseService;

        public JobLevelService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<JobLevel> GetJobLevel()
        {
            var criteria = new GetJobLevelCriteria {};

            return baseService.GetData<JobLevel>(criteria).ToList();
        }

        public CommandResult Insert(InsertJobLevelCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Update(UpdateJobLevelCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int id, int actorId)
        {
            var criteria = new DeleteJobLevelCriteria
            {
                JobLevelId = id,
                ModifiedBy = actorId
            };

            return baseService.ExecuteNonQuery(criteria);
        }
    }
}
