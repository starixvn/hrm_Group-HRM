namespace HRM.Services.Interfaces;

using HRM.Core.Models;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IDepartmentService
{
    IList<Department> GetDepartment(bool? isActive = true);

    Task<IList<Department>> GetDepartmentAsync(bool? isActive = true);

    Department GetById(int departmentId);

    Task<Department> GetByIdAsync(int departmentId);

    CommandResult Delete(int actorId , int id);

    Task<CommandResult> DeleteAsync(int actorId, int id);
}