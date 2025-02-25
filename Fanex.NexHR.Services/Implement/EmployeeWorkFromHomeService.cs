namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Fanex.NexHR.Models;

    public class EmployeeWorkFromHomeService : IEmployeeWorkFromHomeService
    {
        private readonly IBaseService baseService;

        public EmployeeWorkFromHomeService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public EmployeeWorkFromHome GetById(int id)
        {
            var criteria = new GetEmployeeWorkFromHomeByIdCriteria { EmployeeWorkFromHomeId = id };
            return baseService.GetDataEntity<EmployeeWorkFromHome>(criteria);
        }

        public CommandResult Insert(InsertEmployeeWorkFromHomeCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Update(UpdateEmployeeWorkFromHomeCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int id)
        {
            var criteria = new DeleteEmployeeWorkFromHomeCriteria { EmployeeWorkFromHomeId = id };

            return baseService.ExecuteNonQuery(criteria);
        }
    }
}