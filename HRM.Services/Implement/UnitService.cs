using System.Threading.Tasks;

namespace HRM.Services
{
    using System.Collections.Generic;
    using HRM.Criteria.Unit;
    using HRM.Models;
    using System.Linq;
    using HRM.Core.Services;
    using HRM.Core.Models;

    public class UnitService : IUnitService
    {
        private readonly IBaseService baseService;

        public UnitService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public Unit GetById(int id)
        {
            var criteria = new GetUnitByIdCriteria { UnitId = id };

            return baseService.GetDataEntity<Unit>(criteria);
        }

        public async Task<Unit> GetByIdAsync(int id)
        {
            var criteria = new GetUnitByIdCriteria { UnitId = id };

            return await baseService.GetDataEntityAsync<Unit>(criteria);
        }

        public IList<Unit> GetUnit(bool? isActive = true)
        {
            var criteria = new GetUnitsCriteria { IsActive = isActive };

            return baseService.GetData<Unit>(criteria).ToList();
        }

        public async Task<IList<Unit>> GetUnitAsync(bool? isActive = true)
        {
            var criteria = new GetUnitsCriteria { IsActive = isActive };

            var data = await baseService.GetDataAsync<Unit>(criteria);
            return data.ToList();
        }

        public CommandResult Create(InsertUnitCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public async Task<CommandResult> CreateAsync(InsertUnitCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public CommandResult Edit(UpdateUnitCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public async Task<CommandResult> EditAsync(UpdateUnitCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public CommandResult Delete(DeleteUnitCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public async Task<CommandResult> DeleteAsync(DeleteUnitCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}