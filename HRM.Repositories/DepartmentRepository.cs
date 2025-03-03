using System.Threading.Tasks;

namespace HRM.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRM.Criteria.Department;
    using HRM.Models;
    using HRM.Repositories.Interfaces;

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
        public async Task<IList<Department>> GetByActiveAsync(ICriteria criteria)
        {
            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            var departments = await db.QueryAsync<Department, Unit, Department>(
                (d, u) =>
                {
                    d.Unit = u;

                    return d;
                },
                splitOn: "UnitId",
                parameterSet: criteria);

            return departments.ToList();
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

        public async Task<Department> GetByIdAsync(GetDepartmentByIdCriteria criteria)
        {
            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return await db.QueryEntityAsync<Department, Unit, Department>(
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
