using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Criteria.Fnb;
using HRM.Models;
using HRM.Models.Group;

namespace HRM.Services.Interfaces.FnB;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetEmployeesByLastModifiedDate(GetEmployeeByLastModifiedDateCriteria criteria);

    Task<IEnumerable<TeamModel>> GetEmployeeGroupByLastModifiedDate(GetEmployeeGroupByLastModifiedDateCriteria criteria);
}