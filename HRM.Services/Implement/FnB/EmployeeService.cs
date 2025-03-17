using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Fnb;
using HRM.Services.Interfaces.FnB;

namespace HRM.Services.Implement.FnB;

public class EmployeeService(IBaseService baseService) : IEmployeeService
{
    public async Task<CommandResult> GetEmployeesByLastModifiedDate(GetEmployeeByLastModifiedDateCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }
    public async Task<CommandResult> GetEmployeeGroupByLastModifiedDate(GetEmployeeGroupByLastModifiedDateCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}