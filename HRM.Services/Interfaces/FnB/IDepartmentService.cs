using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Models;
using HRM.Models.FnB;

namespace HRM.Services.Interfaces.FnB
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentModel>> GetDepartmentByLastModifiedDate(GetDepartmentByLastModifiedDateCriteria criteria);
    }
}
