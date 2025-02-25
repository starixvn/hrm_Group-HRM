namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Unit;
    using Fanex.NexHR.Models;

    public interface IUnitService
    {
        Unit GetById(int id);

        IList<Unit> GetUnit(bool? isActive = true);

        CommandResult Create(InsertUnitCriteria criteria);
        
        CommandResult Edit(UpdateUnitCriteria criteria);

        CommandResult Delete(DeleteUnitCriteria criteria);

    }
}