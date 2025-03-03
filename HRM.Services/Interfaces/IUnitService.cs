namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.Core.Models;
    using HRM.Criteria.Unit;
    using HRM.Models;

    public interface IUnitService
    {
        Unit GetById(int id);
        Task<Unit> GetByIdAsync(int id);

        IList<Unit> GetUnit(bool? isActive = true);

        Task<IList<Unit>> GetUnitAsync(bool? isActive = true);

        CommandResult Create(InsertUnitCriteria criteria);

        Task<CommandResult> CreateAsync(InsertUnitCriteria criteria);

        CommandResult Edit(UpdateUnitCriteria criteria);

        Task<CommandResult> EditAsync(UpdateUnitCriteria criteria);

        CommandResult Delete(DeleteUnitCriteria criteria);

        Task<CommandResult> DeleteAsync(DeleteUnitCriteria criteria);

    }
}