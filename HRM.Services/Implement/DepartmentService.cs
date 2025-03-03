namespace HRM.Services.Implement;

using HRM.Core.Models;
using HRM.Core.Services;
using Criteria.Department;
using Models;
using HRM.Repositories.Interfaces;
using Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository departmentRepository;
    private readonly IBaseService baseService;

    public DepartmentService(IDepartmentRepository departmentRepository,
        IBaseService baseService)
    {
        this.departmentRepository = departmentRepository;
        this.baseService = baseService;
    }

    public CommandResult Delete(int actorId, int id)
    {
        var criteria = new DeleteDepartmentCriteria
        {
            departmentId = id,
            ModifiedBy = actorId
        };
        return baseService.ExecuteNonQuery(criteria);
    }

    public async Task<CommandResult> DeleteAsync(int actorId, int id)
    {
        var criteria = new DeleteDepartmentCriteria
        {
            departmentId = id,
            ModifiedBy = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public IList<Department> GetDepartment(bool? isActive = true)
    {
        var criteria = new GetDepartmentByActiveCriteria
        {
            IsActive = isActive
        };
        IList<Department> departments = departmentRepository.GetByActive(criteria);

        return departments ?? new List<Department>();
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

    public Department GetById(int departmentId)
    {
        var criteria = new GetDepartmentByIdCriteria
        {
            Id = departmentId
        };
        return departmentRepository.GetById(criteria);
    }

    public async Task<Department> GetByIdAsync(int departmentId)
    {
        var criteria = new GetDepartmentByIdCriteria
        {
            Id = departmentId
        };
        return departmentRepository.GetById(criteria);
    }
}