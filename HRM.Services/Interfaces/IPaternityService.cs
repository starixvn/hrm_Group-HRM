using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Criteria.Paternity;
using HRM.Models;

namespace HRM.Services.Interfaces;

public interface IPaternityService
{
    Task<IList<EmployeePaternity>> GetAll(int year);

    Task<CommandResult> Insert(InsertPaternityCriteria criteria);

    Task<CommandResult> Update(UpdatePaternityCriteria criteria);

    Task<CommandResult> Delete(int id);

    Task<EmployeePaternity> GetById(int id);
}