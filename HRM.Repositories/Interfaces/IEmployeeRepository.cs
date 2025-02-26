namespace HRM.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRM.Models;
    using HRM.Models.EmployeeInformation;

    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployeeByIdAsync(ICriteria criteria);

        Employee GetEmployeeByCode(ICriteria criteria);

        IList<Employee> GetEmployees(ICriteria criteria);

        IList<Employee> GetEmployeesByRole(ICriteria criteria);

        IList<EmployeeFullProfile> GetEmployeeFullInformation(ICriteria criteria);
    }
}