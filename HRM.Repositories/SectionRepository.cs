namespace HRM.Repositories;

using System.Collections.Generic;
using System.Linq;
using Fanex.Data;
using Fanex.Data.Repository;
using Criteria.Section;
using System.Threading.Tasks;
using Models;
using Interfaces;

public class SectionRepository : ISectionRepository
{
    public Section GetById(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var section = db.QueryEntity<Section, Department, Section>(
            (s, d) =>
            {
                s.Department = d;
                return s;
            },
            splitOn: "DepartmentId",
            parameterSet: criteria);

        return section;
    }
    public async Task<Section> GetByIdAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var section = await db.QueryEntityAsync<Section, Department, Section>(
            (s, d) =>
            {
                s.Department = d;
                return s;
            },
            splitOn: "DepartmentId",
            parameterSet: criteria);

        return section;
    }

    public IList<Section> GetSection(GetSectionCriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var sections = db.Query<Section, Department, Section>(
            (s, d) =>
            {
                s.Department = d;

                return s;
            },
            splitOn: "DepartmentId",
            parameterSet: criteria);

        return sections.ToList();
    }

    public async Task<IList<Section>> GetSectionAsync(GetSectionCriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var sections = await db.QueryAsync<Section, Department, Section>(
            (s, d) =>
            {
                s.Department = d;

                return s;
            },
            splitOn: "DepartmentId",
            parameterSet: criteria);

        return sections.ToList();
    }
}