namespace HRM.Services.Interfaces
{
    using HRM.Core.Models;
    using HRM.Models;
    using System.Collections.Generic;

    public interface IDepartmentService
    {
        IList<Department> GetDepartment(bool? isActive = true);

        Department GetById(int departmentId);

        CommandResult Delete(int actorId , int id);
    }
}
