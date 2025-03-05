using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Criteria.Postnatal;
using HRM.Models;

namespace HRM.Services.Interfaces;

public interface IPostnatalService
{
    Task<IList<EmployeePostnatal>> GetAll(int year);

    Task<CommandResult> Insert(InsertPostnatalCriteria criteria);

    Task<CommandResult> Update(UpdatePostnatalCriteria criteria);

    Task<CommandResult> Delete(int id);

    Task<EmployeePostnatal> GetById(int id);
}