namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface IDepartmentService
    {
        IList<Department> GetDepartment(bool? isActive = true);

        Department GetById(int departmentId);

        CommandResult Delete(int actorId , int id);
    }
}
