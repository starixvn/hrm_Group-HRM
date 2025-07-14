namespace HRM.Services.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using Models;

public interface IDepartmentService
{
    Task<IList<Department>> GetDepartmentAsync(bool? isActive = true, int? unitId = null);

    Task<int> GetNextGroupDeptIdAsync();

    Task<Department> GetByIdAsync(int departmentId);

    Task<CommandResult> DeleteAsync(int actorId, int id);

    Task<byte[]> GenerateExcelAsync(IEnumerable<Department> departments);
}