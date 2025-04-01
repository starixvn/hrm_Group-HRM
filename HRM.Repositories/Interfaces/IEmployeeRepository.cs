using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using HRM.Models;
using HRM.Models.EmployeeInformation;

namespace HRM.Repositories.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee> GetEmployeeByIdAsync(ICriteria criteria);

    Task<IEnumerable<Employee>> GetEmployeeAsync(ICriteria criteria);

    Task<IList<Employee>> GetEmployeesByRoleAsync(ICriteria criteria);

    Task<List<EmployeeFullProfile>> GetEmployeeFullInformation(ICriteria criteria);
}