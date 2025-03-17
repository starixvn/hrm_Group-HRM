using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Criteria.Fnb;

namespace HRM.Services.Interfaces.FnB;

public interface IEmployeeService
{
    Task<CommandResult> GetEmployeesByLastModifiedDate(GetEmployeeByLastModifiedDateCriteria criteria);

    Task<CommandResult> GetEmployeeGroupByLastModifiedDate(GetEmployeeGroupByLastModifiedDateCriteria criteria);
}