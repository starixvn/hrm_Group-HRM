namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Fanex.NexHR.Models;

    public interface IEmployeeWorkFromHomeService
    {
        EmployeeWorkFromHome GetById(int id);

        CommandResult Insert(InsertEmployeeWorkFromHomeCriteria criteria);

        CommandResult Update(UpdateEmployeeWorkFromHomeCriteria criteria);

        CommandResult Delete(int id);
    }
}