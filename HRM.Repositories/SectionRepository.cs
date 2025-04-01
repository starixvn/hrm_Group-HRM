using HRM.Models.Section;

namespace HRM.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Criteria.Section;
using Fanex.Data;
using Fanex.Data.Repository;
using Interfaces;
using Models;

public class SectionRepository : ISectionRepository
{
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