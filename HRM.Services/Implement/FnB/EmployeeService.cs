using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Services;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Services.Interfaces.FnB;

namespace HRM.Services.Implement.FnB;

public class EmployeeService(IBaseService baseService) : IEmployeeService
{
    public async Task<IEnumerable<EmployeeModel>> GetEmployeesByLastModifiedDate(GetEmployeeByLastModifiedDateCriteria criteria)
    {
        return await baseService.GetDataAsync<EmployeeModel>(criteria);
    }

    public async Task<IEnumerable<EmployeeModel>> GetEmployeeSecondments(GetEmployeeSecondmentCriteria criteria)
    {
        return await baseService.GetDataAsync<EmployeeModel>(criteria);
    }

    public async Task<IEnumerable<TeamModel>> GetEmployeeTeamByLastModifiedDate(GetEmployeeTeamByLastModifiedDateCriteria criteria)
    {
        return await baseService.GetDataAsync<TeamModel>(criteria);
    }

    public async Task<IEnumerable<EmployeeCalendarModel>> GetEmployeeCalendarByLastModifiedDate(GetEmployeeCalendarByLastModifiedDateCriteria criteria)
    {
        return await baseService.GetDataAsync<EmployeeCalendarModel>(criteria);
    }
}