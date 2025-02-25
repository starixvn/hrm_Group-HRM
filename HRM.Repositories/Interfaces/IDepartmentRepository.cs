namespace HRM.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using HRM.Criteria.Department;
    using HRM.Models;
    using System.Collections.Generic;

    public interface IDepartmentRepository
    {
        IList<Department> GetByActive(ICriteria criteria);
        Department GetById(GetDepartmentByIdCriteria criteria);
    }
}