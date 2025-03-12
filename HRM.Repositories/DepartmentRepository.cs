using HRM.Models.Unit;

namespace HRM.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Criteria.Department;
using Fanex.Data;
using Fanex.Data.Repository;
using Interfaces;
using Models;

public class DepartmentRepository : IDepartmentRepository
{
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