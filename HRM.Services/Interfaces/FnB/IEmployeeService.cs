using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;

namespace HRM.Services.Interfaces.FnB;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeModel>> GetEmployeesByLastModifiedDate(GetEmployeeByLastModifiedDateCriteria criteria);

    Task<IEnumerable<EmployeeModel>> GetEmployeeSecondments(GetEmployeeSecondmentCriteria criteria);

    Task<IEnumerable<TeamModel>> GetEmployeeTeamByLastModifiedDate(GetEmployeeTeamByLastModifiedDateCriteria criteria);

    Task<IEnumerable<EmployeeCalendarModel>> GetEmployeeCalendarByLastModifiedDate(
        GetEmployeeCalendarByLastModifiedDateCriteria criteria);
}