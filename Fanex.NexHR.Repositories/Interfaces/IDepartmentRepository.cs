namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface IDepartmentRepository
    {
        IList<Department> GetByActive(ICriteria criteria);
        Department GetById(GetDepartmentByIdCriteria criteria);
    }
}