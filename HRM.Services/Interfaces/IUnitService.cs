namespace HRM.Services
{
    using System.Collections.Generic;
    using HRM.Core.Models;
    using HRM.Criteria.Unit;
    using HRM.Models;

    public interface IUnitService
    {
        Unit GetById(int id);

        IList<Unit> GetUnit(bool? isActive = true);

        CommandResult Create(InsertUnitCriteria criteria);
        
        CommandResult Edit(UpdateUnitCriteria criteria);

        CommandResult Delete(DeleteUnitCriteria criteria);

    }
}