using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Models.Section;

namespace HRM.Services.Interfaces.FnB
{
    public interface ISectionService
    {
        Task<IEnumerable<SectionModel>> GetSectionsByLastModifiedDate(GetSectionByLastModifiedDateCriteria criteria);
    }
}
