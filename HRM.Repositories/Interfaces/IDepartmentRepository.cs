namespace HRM.Repositories.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using Criteria.Department;
using Fanex.Data.Repository;
using Models;

public interface IDepartmentRepository
{
    Task<IList<Department>> GetByActiveAsync(ICriteria criteria);
    
    Task<Department> GetByIdAsync(GetDepartmentByIdCriteria criteria);
}