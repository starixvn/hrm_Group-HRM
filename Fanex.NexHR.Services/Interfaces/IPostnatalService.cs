namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Core.Models;
    using Criteria.Postnatal;
    using Fanex.NexHR.Models;

    public interface IPostnatalService
    {
        IList<EmployeePostnatal> GetAll(int year);

        CommandResult Insert(InsertPostnatalCriteria criteria);

        CommandResult Update(UpdatePostnatalCriteria criteria);

        CommandResult Delete(int id);

        EmployeePostnatal GetById(int id);
    }
}