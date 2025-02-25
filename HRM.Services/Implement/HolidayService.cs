namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.Holidays;
    using HRM.Models;

    public class HolidayService : IHolidayService
    {
        private readonly IBaseService baseService;

        public HolidayService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<Holiday> GetByYear(int year)
        {
            var criteria = new GetHolidaysCriteria
            {
                Year = year
            };
            IList<Holiday> holidays = baseService.GetData<Holiday>(criteria).ToList();

            return holidays ?? new List<Holiday>();
        }

        public Holiday GetById(int holidayId)
        {
            var criteria = new GetHolidayByIdCriteria
            {
                Id = holidayId
            };
            Holiday holiday = baseService.GetDataEntity<Holiday>(criteria);

            return holiday ?? new Holiday();
        }

        public CommandResult Create(int actorId, Holiday model)
        {
            var criteria = new CreateHolidayCriteria
            {
                HolidayName = model.HolidayName,
                TypeId = model.TypeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                ModifiedBy = actorId,
                ModifiedDate = DateTimeOffset.Now.DateTime
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Edit(int actorId, Holiday model)
        {
            var criteria = new EditHolidayCriteria
            {
                Id = model.Id,
                HolidayName = model.HolidayName,
                TypeId = model.TypeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                ModifiedBy = actorId,
                ModifiedDate = DateTimeOffset.Now.DateTime
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int actorId, int id)
        {
            var criteria = new DeleteHolidayCriteria
            {
                Id = id,
                ModifiedBy = actorId,
                ModifiedDate = DateTimeOffset.Now.DateTime
            };
            return baseService.ExecuteNonQuery(criteria);
        }
    }
}