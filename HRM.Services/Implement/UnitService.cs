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

        public IList<Unit> GetUnit(bool? isActive = true)
        {
            var criteria = new GetUnitsCriteria { IsActive = isActive };

            return baseService.GetData<Unit>(criteria).ToList();
        }

        public CommandResult Create(InsertUnitCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Edit(UpdateUnitCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(DeleteUnitCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }
    }
}