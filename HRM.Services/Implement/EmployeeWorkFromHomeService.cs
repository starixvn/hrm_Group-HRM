namespace HRM.Services;

using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.EmployeeWorkFromHome;
using HRM.Models;
using HRM.Services.Interfaces;

public class EmployeeWorkFromHomeService(IBaseService baseService) : IEmployeeWorkFromHomeService
{
    public async Task<EmployeeWorkFromHome> GetByIdAsync(int id)
    {
        var criteria = new GetEmployeeWorkFromHomeByIdCriteria { EmployeeWorkFromHomeId = id };
        return await baseService.GetDataEntityAsync<EmployeeWorkFromHome>(criteria);
    }

    public async Task<CommandResult> CreateAsync(InsertEmployeeWorkFromHomeCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> UpdateAsync(UpdateEmployeeWorkFromHomeCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> DeleteAsync(int id)
    {
        var criteria = new DeleteEmployeeWorkFromHomeCriteria { EmployeeWorkFromHomeId = id };

        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}