namespace HRM.Services
{
    using System.Collections.Generic;
    using Core.Models;
    using Criteria.Paternity;
    using HRM.Models;

    public interface IPaternityService
    {
        IList<EmployeePaternity> GetAll(int year);

        CommandResult Insert(InsertPaternityCriteria criteria);

        CommandResult Update(UpdatePaternityCriteria criteria);

        CommandResult Delete(int id);

        EmployeePaternity GetById(int id);
    }
}