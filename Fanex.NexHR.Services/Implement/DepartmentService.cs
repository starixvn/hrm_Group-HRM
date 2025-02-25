namespace Fanex.NexHR.Services.Implement
{
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;

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

        public IList<Department> GetDepartment(bool? isActive = true)
        {
            var criteria = new GetDepartmentByActiveCriteria
            {
                IsActive = isActive
            };
            IList<Department> departments = departmentRepository.GetByActive(criteria);

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
    }
}
