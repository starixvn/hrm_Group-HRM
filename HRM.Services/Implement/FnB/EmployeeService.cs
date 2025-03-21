using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Fnb;
using HRM.Models;
using HRM.Models.Group;
using HRM.Services.Interfaces.FnB;

namespace HRM.Services.Implement.FnB;

public class EmployeeService(IBaseService baseService) : IEmployeeService
{
    public async Task<IEnumerable<Employee>> GetEmployeesByLastModifiedDate(GetEmployeeByLastModifiedDateCriteria criteria)
    {
        return await baseService.GetDataAsync<Employee>(criteria);
    }
    public async Task<IEnumerable<TeamModel>> GetEmployeeGroupByLastModifiedDate(GetEmployeeGroupByLastModifiedDateCriteria criteria)
    {
        return await baseService.GetDataAsync<TeamModel>(criteria);
    }
}