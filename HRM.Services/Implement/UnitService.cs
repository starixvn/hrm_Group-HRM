namespace HRM.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using Criteria.Unit;
using Models;
using GO.UserControl.Client.Core.Session;
using Core.Constants;

public class UnitService(IBaseService baseService,
    IUserControlSession userControlSession) : IUnitService
{
    public async Task<Unit> GetByIdAsync(int id)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetUnitByIdCriteria
        {
            UnitId = id, 
            CompanyId = companyId
        };

        return await baseService.GetDataEntityAsync<Unit>(criteria);
    }

    public async Task<IList<Unit>> GetUnitAsync(bool? isActive = true)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetUnitsCriteria
        {
            IsActive = isActive, 
            CompanyId = companyId
        };

        var data = await baseService.GetDataAsync<Unit>(criteria);
        return data.ToList();
    }

    public async Task<CommandResult> CreateAsync(InsertUnitCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> EditAsync(UpdateUnitCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> DeleteAsync(DeleteUnitCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}