namespace HRM.Repositories.Interfaces;

using Fanex.Data.Repository;
using Criteria.Department;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IDepartmentRepository
{
    IList<Department> GetByActive(ICriteria criteria);

    Task<IList<Department>> GetByActiveAsync(ICriteria criteria);

    Department GetById(GetDepartmentByIdCriteria criteria);

    Task<Department> GetByIdAsync(GetDepartmentByIdCriteria criteria);
}