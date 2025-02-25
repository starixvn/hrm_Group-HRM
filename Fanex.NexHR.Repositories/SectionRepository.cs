namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.Section;
    using Fanex.NexHR.Models;

    public class SectionRepository : ISectionRepository
    {
        public Section GetById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
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
        }

        public IList<Section> GetSection(GetSectionCriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
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
        }
    }
}