namespace HRM.Services
{
    using HRM.Core.Models;
    using HRM.Criteria.EmployeeWorkFromHome;
    using HRM.Models;

    public interface IEmployeeWorkFromHomeService
    {
        EmployeeWorkFromHome GetById(int id);

        CommandResult Insert(InsertEmployeeWorkFromHomeCriteria criteria);

        CommandResult Update(UpdateEmployeeWorkFromHomeCriteria criteria);

        CommandResult Delete(int id);
    }
}