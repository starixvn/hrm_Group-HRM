using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Models.Section;
using HRM.Models.Unit;
using HRM.Services.Interfaces.FnB;

namespace HRM.Services.Implement.FnB
{
    public class UnitService(IBaseService baseService) : IUnitService
    {
        public async Task<IEnumerable<UnitModel>> GetUnitByLastModifiedDate(GetUnitByLastModifiedDateCriteria criteria)
        {
            return await baseService.GetDataAsync<UnitModel>(criteria);
        }
    }
}
