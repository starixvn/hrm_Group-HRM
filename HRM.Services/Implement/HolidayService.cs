using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Holidays;
using HRM.Models;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class HolidayService(IBaseService baseService,
    IUserControlSession userControlSession) : IHolidayService
{
    public async Task<IEnumerable<Holiday>> GetByYearAsync(int year)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);

        var criteria = new GetHolidaysCriteria
        {
            Year = year,
            CompanyId = companyId
        };
        return await baseService.GetDataAsync<Holiday>(criteria);
    }

    public async Task<Holiday> GetByIdAsync(int holidayId)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);

        var criteria = new GetHolidayByIdCriteria
        {
            Id = holidayId,
            CompanyId = companyId
        };
        return await baseService.GetDataEntityAsync<Holiday>(criteria);
    }

    public async Task<CommandResult> CreateAsync(int actorId, Holiday model)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new CreateHolidayCriteria
        {
            HolidayName = model.HolidayName,
            TypeId = model.TypeId,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            ModifiedBy = actorId,
            ModifiedDate = DateTimeOffset.Now.DateTime,
            CompanyId = companyId
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