using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Services;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Services.Interfaces.FnB;

namespace HRM.Services.Implement.FnB;

public class DepartmentService(IBaseService baseService) : IDepartmentService
{
    public async Task<IEnumerable<DepartmentModel>> GetDepartmentByLastModifiedDate(GetDepartmentByLastModifiedDateCriteria criteria)
    {
        return await baseService.GetDataAsync<DepartmentModel>(criteria);
    }
}