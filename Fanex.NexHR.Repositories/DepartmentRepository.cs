namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    public class DepartmentRepository : IDepartmentRepository

    {
        public IList<Department> GetByActive(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var departments = db.Query<Department, Unit, Department>(
                        (d, u) =>
                        {
                            d.Unit = u;

                            return d;
                        },
                        splitOn: "UnitId",
                        parameterSet: criteria);

                return departments.ToList();
            }
        }

        public Department GetById(GetDepartmentByIdCriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<Department, Unit, Department>(
                    (d, u) =>
                    {
                        d.Unit = u;

                        return d;
                    },
                    "UnitId",
                    criteria);
            }
        }
    }
}
