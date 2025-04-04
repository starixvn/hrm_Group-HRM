namespace HRM.Services.Implement;

using System.Collections.Generic;
using System.Threading.Tasks;
using Criteria.Department;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Repositories.Interfaces;
using Interfaces;
using Models;

public class DepartmentService(
    IDepartmentRepository departmentRepository,
    IBaseService baseService) : IDepartmentService
{
    public async Task<CommandResult> DeleteAsync(int actorId, int id)
    {
        var criteria = new DeleteDepartmentCriteria
        {
            departmentId = id,
            ModifiedBy = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<IList<Department>> GetDepartmentAsync(bool? isActive = true)
    {
        var criteria = new GetDepartmentByActiveCriteria
        {
            IsActive = isActive
        };
        var departments = await departmentRepository.GetByActiveAsync(criteria);

        return departments ?? new List<Department>();
    }

    public async Task<Department> GetByIdAsync(int departmentId)
    {
        var criteria = new GetDepartmentByIdCriteria
        {
            Id = departmentId
        };
        return await departmentRepository.GetByIdAsync(criteria);
    }
}