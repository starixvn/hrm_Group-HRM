using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Fnb;
using HRM.Models.Section;
using HRM.Services.Interfaces.FnB;

namespace HRM.Services.Implement.FnB
{
    public class SectionService(IBaseService baseService) : ISectionService
    {
        public async Task<IEnumerable<Section>> GetSectionsByLastModifiedDate(GetSectionByLastModifiedDate criteria)
        {
            return await baseService.GetDataAsync<Section>(criteria);
        }
    }
}
