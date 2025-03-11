using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Paternity;
using HRM.Models;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class PaternityService(IBaseService baseService) : IPaternityService
{
    public async Task<CommandResult> Delete(int id)
    {
        var criteria = new DeletePaternityCriteria { PaternityId = id };

        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<IList<EmployeePaternity>> GetAll(int year, short companyId)
    {
        var criteria = new GetPaternityCriteria
        {
            Year = year,
            CompanyId = companyId
        };
        IList<EmployeePaternity> paternityList = null;
        if (criteria.IsValid())
        {
            paternityList = (await baseService.GetDataAsync<EmployeePaternity>(criteria)).ToList();
        }

        return paternityList ?? new List<EmployeePaternity>();
    }

    public async Task<EmployeePaternity> GetById(int id)
    {
        var criteria = new GetPaternityByIdCriteria { PaternityId = id };

        EmployeePaternity postnatal = null;
        if (criteria.IsValid())
        {
            postnatal = await baseService.GetDataEntityAsync<EmployeePaternity>(criteria);
        }
        return postnatal ?? new EmployeePaternity();
    }

    public async Task<CommandResult> Insert(InsertPaternityCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> Update(UpdatePaternityCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}