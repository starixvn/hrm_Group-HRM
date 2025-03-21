using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Services;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Services.Interfaces.FnB;

namespace HRM.Services.Implement.FnB;

public class ShiftService(IBaseService baseService) : IShiftService
{
    public async Task<IEnumerable<ShiftModel>> GetShiftByLastModifiedDate(GetShiftByLastModifiedDateCriteria criteria)
    {
        return await baseService.GetDataAsync<ShiftModel>(criteria);
    }
}