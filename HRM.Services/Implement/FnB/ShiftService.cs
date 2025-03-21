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
using HRM.Models.Shifts;
using HRM.Services.Interfaces.FnB;

namespace HRM.Services.Implement.FnB
{
    public class ShiftService(IBaseService baseService) : IShiftService
    {
        public async Task<IEnumerable<ShiftModel>> GetShiftByLastModifiedDate(GetShiftByLastModifiedDateCriteria criteria)
        {
            return await baseService.GetDataAsync<ShiftModel>(criteria);
        }
    }
}
