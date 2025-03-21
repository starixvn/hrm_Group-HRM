using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Models;
using HRM.Models.FnB;
using HRM.Models.Unit;

namespace HRM.Services.Interfaces.FnB
{
    public interface IUnitService
    {
        Task<IEnumerable<UnitModel>> GetUnitByLastModifiedDate(GetUnitByLastModifiedDateCriteria criteria);
    }
}
