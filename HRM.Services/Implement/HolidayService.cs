using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Holidays;
using HRM.Models;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class HolidayService(IBaseService baseService) : IHolidayService
{
    public async Task<IEnumerable<Holiday>> GetByYearAsync(int year)
    {
        var criteria = new GetHolidaysCriteria
        {
            Year = year
        };
        return await baseService.GetDataAsync<Holiday>(criteria);
    }

    public async Task<Holiday> GetByIdAsync(int holidayId)
    {
        var criteria = new GetHolidayByIdCriteria
        {
            Id = holidayId
        };
        return await baseService.GetDataEntityAsync<Holiday>(criteria);
    }

    public async Task<CommandResult> CreateAsync(int actorId, Holiday model)
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
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> EditAsync(int actorId, Holiday model)
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
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> DeleteAsync(int actorId, int id)
    {
        var criteria = new DeleteHolidayCriteria
        {
            Id = id,
            ModifiedBy = actorId,
            ModifiedDate = DateTimeOffset.Now.DateTime
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}