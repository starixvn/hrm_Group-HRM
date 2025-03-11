using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Postnatal;
using HRM.Models;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class PostnatalService(IBaseService baseService) : IPostnatalService
{
    public async Task<IList<EmployeePostnatal>> GetAll(int year, short companyId)
    {
        var criteria = new GetPostnatalCriteria
        {
            Year = year, 
            CompanyId = companyId
        };
        return (await baseService.GetDataAsync<EmployeePostnatal>(criteria)).ToList();
    }

    public async Task<EmployeePostnatal> GetById(int id)
    {
        var criteria = new GetPostnatalByIdCriteria { PostnatalId = id };
        return await baseService.GetDataEntityAsync<EmployeePostnatal>(criteria);
    }

    public async Task<CommandResult> Insert(InsertPostnatalCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> Update(UpdatePostnatalCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> Delete(int id)
    {
        var criteria = new DeletePostnatalCriteria { PostnatalId = id };

        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}