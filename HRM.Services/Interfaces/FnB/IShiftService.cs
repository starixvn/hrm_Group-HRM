using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;

namespace HRM.Services.Interfaces.FnB;

public interface IShiftService
{
    Task<IEnumerable<ShiftModel>> GetAllShift(GetAllShiftCriteria criteria);
}